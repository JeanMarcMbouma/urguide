using IdentityModel;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddUrGuideAuthServices(this IServiceCollection services, IConfiguration configuration)
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

             
            })
            .AddApiAuthorization<UrGuideUser, UrGuideAuthContext>(options =>
            {
                options.Clients.Add(new IdentityServer4.Models.Client
                {
                    ClientName = "UrGuide.WebAPI",
                    ClientId = "UrGuide.WebAPI",
                    AllowedScopes = { StandardScopes.OpenId },
                    AllowedGrantTypes = { GrantTypes.ClientCredentials },
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret = false,
                    ClientSecrets = { new IdentityServer4.Models.Secret("secret".ToSha256()) }
                });

            });

            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IInstantMessagingService, InstantMessagingService>();
            /*
             ,
          "UrGuide.WebAPI": {
            "Profile": "SPA",
            "RedirectUri": "https://localhost:5001/swagger/oauth2-redirect.html",
            "LogoutUri": "https://localhost:5001/swagger/oauth2-logout.html",
            "ResponseType" :  "code",
            "Scope": "profile openid"
          }
             */
            services.AddAuthentication()
                .AddIdentityServerJwt();
            services.AddSignalR();
            return services;
        }
    }
}
