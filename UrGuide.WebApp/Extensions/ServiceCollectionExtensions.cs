using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
using static Duende.IdentityModel.OidcConstants;
using static Duende.IdentityServer.Models.IdentityResources;

namespace UrGuide.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrGuideAuthServices(this IServiceCollection services, 
            IConfiguration configuration)
        {
            services.Configure<IPStackConfiguration>(configuration.GetSection("IpStack"));
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                               .ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEmailSender, EmailService>();
            services.AddTransient<IWebHelper, WebHelper>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddTransient<IIPStackService, IPStackService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();
            services.AddScoped<IPasskeyService, PasskeyService>();
            
            // Configure Fido2 for Passkey/WebAuthn support
            string applicationUri = configuration.GetValue<string>("ApplicationUri") ?? "https://localhost:5001";
            services.AddFido2(options =>
            {
                options.ServerDomain = new Uri(applicationUri).Host;
                options.ServerName = "UrGuide";
                options.Origins = new HashSet<string> { applicationUri };
                options.TimestampDriftTolerance = 300000; // 5 minutes
            });
            
            services.AddDbContext<UrGuideAuthContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Id4")));

            services.AddDefaultIdentity<UrGuideUser>(options => options.SignIn.RequireConfirmedAccount = true)
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
            string xamarin = configuration.GetValue<string>("Xamarin");
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
                        new Secret(xamarinClientSecret.Sha256())
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
