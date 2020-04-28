using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
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
                        RedirectUris = { "https://localhost:5001/swagger/oauth2-redirect.html" },
                        ClientName = "urguide_swagger_ui",
                        ClientId = "urguide_swagger_ui",
                        AllowedScopes = { IdentityServer4.IdentityServerConstants.StandardScopes.OpenId },
                        AllowedGrantTypes = { GrantTypes.Implicit },
                        RequireConsent = false,
                        AllowAccessTokensViaBrowser = true
                    });
                });

            services.AddAuthentication()
                .AddIdentityServerJwt();

            return services;
        }
    }
}
