using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            services.AddScoped<IAdminSeedingService, AdminSeedingService>();

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
            .AddDeveloperSigningCredential();

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IInstantMessagingService, InstantMessagingService>();

            services.AddAuthentication()
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = applicationUri;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

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
