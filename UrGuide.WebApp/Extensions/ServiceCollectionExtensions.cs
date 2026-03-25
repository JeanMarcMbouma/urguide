using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using AspNet.Security.OAuth.Apple;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UrGuide.Shared.Configuration;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Entities;
using UrGuide.WebApp.Services;

namespace UrGuide.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrGuideAuthServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<IPStackConfiguration>(configuration.GetSection("IpStack"));
            services.AddHttpContextAccessor();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var accessor = factory.GetRequiredService<IHttpContextAccessor>();
                var endpoint = accessor.HttpContext?.GetEndpoint();
                var actionDescriptor = endpoint?.Metadata.GetMetadata<ActionDescriptor>();
                var urlHelperFactory = factory.GetService<IUrlHelperFactory>();
                return urlHelperFactory?.GetUrlHelper(new ActionContext(
                    accessor.HttpContext!,
                    new Microsoft.AspNetCore.Routing.RouteData(
                        new RouteValueDictionary(actionDescriptor?.RouteValues ?? new Dictionary<string, string?>())
                        ),
                        actionDescriptor!
                    ))
                    ?? throw new InvalidOperationException("Unable to resolve IUrlHelper.");
            });
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<IWebHelper, WebHelper>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddTransient<IIPStackService, IPStackService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            services.AddScoped<IPasskeyService, PasskeyService>();
            services.AddScoped<UrGuide.Services.Contracts.IAdminService, AdminService>();
            services.AddSingleton<AdminPlatformSettings>();
            services.AddScoped<IAdminSeedingService, AdminSeedingService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ISocialAuthService, SocialAuthService>();

            // Configure Fido2 for Passkey/WebAuthn support
            string applicationUri = configuration.GetValue<string>("ApplicationUri") ?? "https://localhost:5001";
            services.AddFido2(options =>
            {
                options.ServerDomain = new Uri(applicationUri).Host;
                options.ServerName = "UrGuide";
                options.Origins = new HashSet<string> { applicationUri };
                options.TimestampDriftTolerance = 300000; // 5 minutes
            });

            // Configure Data Protection for encrypting sensitive data (e.g., 2FA secrets)
            // Keys are persisted to the file system for production use
            // In production, consider using Azure Key Vault or other secure key storage
            var dataProtectionPath = configuration.GetValue<string>("DataProtection:KeyPath")
                ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "keys");

            services.AddDataProtection()
                .SetApplicationName("UrGuide")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(dataProtectionPath));

            var authConnectionString = configuration.GetConnectionString("AuthConnection")
                ?? configuration.GetConnectionString("Id4");

            services.AddDbContext<UrGuideAuthContext>(options =>
                options.UseSqlServer(authConnectionString));

            services.AddDefaultIdentity<UrGuideUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UrGuideAuthContext>();

            services.AddIdentityServer(options =>
            {
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                options.IssuerUri = applicationUri;
            })
            .AddInMemoryIdentityResources(GetIdentityResources())
            .AddInMemoryApiScopes(GetApiScopes())
            .AddInMemoryClients(GetClients(configuration, applicationUri))
            .AddAspNetIdentity<UrGuideUser>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddDeveloperSigningCredential();

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IInstantMessagingService, InstantMessagingService>();

            // Configure JWT authentication to support both IdentityServer tokens and custom JWT tokens
            var jwtKey = configuration.GetValue<string>("Jwt:Key");
            if (string.IsNullOrEmpty(jwtKey))
            {
                jwtKey = $"UrGuide_JWT_Secret_Key_{applicationUri}_Development_Only";
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
                options.DefaultScheme = "Bearer";
            })
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = applicationUri;
                    
                    // Allow HTTP for development (localhost)
                    // In production, always use HTTPS
                    if (applicationUri.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    {
                        options.RequireHttpsMetadata = false;
                    }
                    
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = true,
                        ValidIssuer = applicationUri,
                        
                        // Map JWT claim names to standard ASP.NET claim types
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                    
                    // Transform JWT claims to standard ClaimTypes for consistent usage
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            if (context.Principal?.Identity is System.Security.Claims.ClaimsIdentity identity)
                            {
                                // Map "sub" to NameIdentifier
                                var subClaim = identity.FindFirst("sub");
                                if (subClaim != null && !identity.HasClaim(System.Security.Claims.ClaimTypes.NameIdentifier, subClaim.Value))
                                {
                                    identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, subClaim.Value));
                                }

                                // Map "email" to Email
                                var emailClaim = identity.FindFirst("email");
                                if (emailClaim != null && !identity.HasClaim(System.Security.Claims.ClaimTypes.Email, emailClaim.Value))
                                {
                                    identity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, emailClaim.Value));
                                }
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // Register social login providers (Google, Apple, Microsoft)
            // Credentials are loaded from configuration; providers are only registered when configured
            var googleClientId = configuration.GetValue<string>("SocialAuth:Google:ClientId");
            var googleClientSecret = configuration.GetValue<string>("SocialAuth:Google:ClientSecret");
            if (!string.IsNullOrEmpty(googleClientId) && !string.IsNullOrEmpty(googleClientSecret))
            {
                services.AddAuthentication()
                    .AddGoogle("Google", options =>
                    {
                        options.ClientId = googleClientId;
                        options.ClientSecret = googleClientSecret;
                        options.Scope.Add("email");
                        options.Scope.Add("profile");
                        options.SaveTokens = true;
                        options.Events.OnCreatingTicket = context =>
                        {
                            var picture = context.User.GetProperty("picture").GetString();
                            if (!string.IsNullOrEmpty(picture))
                            {
                                context.Identity?.AddClaim(new Claim("picture", picture));
                            }
                            return Task.CompletedTask;
                        };
                    });
            }

            var microsoftClientId = configuration.GetValue<string>("SocialAuth:Microsoft:ClientId");
            var microsoftClientSecret = configuration.GetValue<string>("SocialAuth:Microsoft:ClientSecret");
            if (!string.IsNullOrEmpty(microsoftClientId) && !string.IsNullOrEmpty(microsoftClientSecret))
            {
                services.AddAuthentication()
                    .AddMicrosoftAccount("Microsoft", options =>
                    {
                        options.ClientId = microsoftClientId;
                        options.ClientSecret = microsoftClientSecret;
                        // Support both personal Microsoft accounts and Azure AD (work/school)
                        options.AuthorizationEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
                        options.TokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
                        options.SaveTokens = true;
                    });
            }

            var appleClientId = configuration.GetValue<string>("SocialAuth:Apple:ClientId");
            var appleTeamId = configuration.GetValue<string>("SocialAuth:Apple:TeamId");
            var appleKeyId = configuration.GetValue<string>("SocialAuth:Apple:KeyId");
            var applePrivateKey = configuration.GetValue<string>("SocialAuth:Apple:PrivateKey");
            if (!string.IsNullOrEmpty(appleClientId) && !string.IsNullOrEmpty(appleTeamId)
                && !string.IsNullOrEmpty(appleKeyId) && !string.IsNullOrEmpty(applePrivateKey))
            {
                services.AddAuthentication()
                    .AddApple("Apple", options =>
                    {
                        options.ClientId = appleClientId;
                        options.TeamId = appleTeamId;
                        options.KeyId = appleKeyId;
                        options.GenerateClientSecret = true;
                        options.SaveTokens = true;
                    });
            }

            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, SignalRAuthPostConfigureOptions>());
            return services;
        }

        private static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        private static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope("api1", "UrGuide API")
                {
                    UserClaims = { JwtClaimTypes.Role, JwtClaimTypes.Name, JwtClaimTypes.Email }
                }
            };
        }

        private static IEnumerable<Client> GetClients(IConfiguration configuration, string applicationUri)
        {
            string xamarin = configuration.GetValue<string>("Xamarin", "");
            // Read client secret from configuration (User Secrets or Azure Key Vault)
            // Default to empty string if not configured to fail securely
            string xamarinClientSecret = configuration.GetValue<string>("IdentityServer:Clients:Xamarin:ClientSecret") ?? "";

            return new Client[]
            {
                new Client
                {
                    ClientName = "UrGuide.WebAPI",
                    ClientId = "UrGuide.WebAPI",
                    AllowedGrantTypes = Duende.IdentityServer.Models.GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = {
                        $"{applicationUri}/swagger/oauth2-redirect.html",
                        "https://localhost:5001/swagger/oauth2-redirect.html"
                    },
                    PostLogoutRedirectUris = {
                        $"{applicationUri}/swagger/" ,
                        $"https://localhost:5001/swagger/" ,
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                },
                new Client
                {
                    ClientId = configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientId") ?? "admin-dashboard",
                    ClientName = "UrGuide Admin Dashboard",
                    AllowedGrantTypes = Duende.IdentityServer.Models.GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Duende.IdentityServer.Models.Secret(
                            (configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientSecret") 
                            ?? throw new InvalidOperationException("Admin dashboard client secret must be configured")).Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
                    },
                    AllowOfflineAccess = true, // Enable refresh tokens
                    AccessTokenLifetime = 28800, // 8 hours (matches JWT config)
                    RefreshTokenUsage = Duende.IdentityServer.Models.TokenUsage.ReUse,
                    RefreshTokenExpiration = Duende.IdentityServer.Models.TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 604800, // 7 days
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                new Client
                {
                    ClientId = "xamarin",
                    ClientName = "UrGuide Xamarin OpenId Client",
                    AllowedGrantTypes = Duende.IdentityServer.Models.GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Duende.IdentityServer.Models.Secret(xamarinClientSecret.Sha256())
                    },
                    RedirectUris = { xamarin },
                    RequireConsent = false,
                    RequirePkce = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    PostLogoutRedirectUris = { xamarin },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api1"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                }
            };
        }
    }
}
