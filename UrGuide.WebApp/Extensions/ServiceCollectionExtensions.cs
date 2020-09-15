using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using UrGuide.Shared.Configuration;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Entities;
using UrGuide.WebApp.Services;
using static IdentityModel.OidcConstants;

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
            services.AddTransient<IWebHelper, WebHelper>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddScoped<IUserContext, UserContext>();
            services.AddTransient<IIPStackService, IPStackService>();
            services.AddDbContext<UrGuideAuthContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Id4")));

            services.AddDefaultIdentity<UrGuideUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<UrGuideAuthContext>();

            services.AddIdentityServer(options =>
            {
                options.UserInteraction.LoginUrl = "/sign-in";
                options.UserInteraction.LogoutUrl = "/account/logout";
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);
                options.IssuerUri = "https://urguide.azurewebsites.net";
            })
            .AddApiAuthorization<UrGuideUser, UrGuideAuthContext>(options =>
            {
                options.Clients.Add(new Client
                {
                    ClientName = "UrGuide.WebAPI",
                    ClientId = "UrGuide.WebAPI",
                    AllowedScopes = { StandardScopes.OpenId },
                    AllowedGrantTypes = { OidcConstants.GrantTypes.ClientCredentials },
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,
                    ClientSecrets = { new Secret("secret".ToSha256()) }
                });
                string xamarin = configuration.GetValue<string>("Xamarin");
                options.Clients.Add(new Client
                {
                    ClientId = "xamarin",
                    ClientName = "UrGuide Xamarin OpenId Client",
                    AllowedGrantTypes = IdentityServer4.Models.GrantTypes.Hybrid,
                    //Used to retrieve the access token on the back channel.
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { xamarin },
                    RequireConsent = false,
                    RequirePkce = true,
                    PostLogoutRedirectUris = { $"{xamarin}/Account/Redirecting" },
                    AllowedScopes = 
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    //Allow requesting refresh tokens for long lived API access
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true
                });
            });

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IInstantMessagingService, InstantMessagingService>();

            services.AddAuthentication()
                .AddIdentityServerJwt();
            
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>, SignalRAuthPostConfigureOptions>()); 
            return services;
        }
    }
}
