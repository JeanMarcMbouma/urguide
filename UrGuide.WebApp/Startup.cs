using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using UrGuide.WebApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using UrGuide.WebApp.Extensions;
using UrGuide.Services.Extensions;
using FluentValidation.AspNetCore;
using AspNetCoreRateLimit;
using System.Collections.Generic;
using static IdentityModel.OidcConstants;
using IdentityModel;
using Microsoft.AspNetCore.HttpOverrides;

namespace UrGuide.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddUrGuideAuthServices(Configuration);

            services.AddUrGuideServices(Configuration);

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddControllersWithViews(options => {
                options.Filters.Add(typeof(Filters.EnvelopResultFilter));
                options.Filters.Add(typeof(Filters.SaveChangeActionFilter));
            }).AddFluentValidation();
            services.AddRazorPages();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                            TokenUrl = new Uri("/connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                { StandardScopes.OpenId, "UrGuide Web API" },
                                { StandardScopes.Profile, "User's profile" }
                            }
                        }
                    }
                });
                c.OperationFilter<Filters.AuthorizeCheckOperationFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ur Guide API", Version = "v1" });
            });
            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            serviceProvider.GetRequiredService<UrGuideAuthContext>().Database.Migrate();
            serviceProvider.GetRequiredService<UrGuide.Data.UrGuideContext>().Database.Migrate();

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();
            app.UseForwardedHeaders(new ForwardedHeadersOptions { 
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ur Guide API v1");
                c.OAuthClientId("UrGuide.WebAPI");
                c.OAuthClientSecret("secret".ToSha256());
            });
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                spa.Options.StartupTimeout = TimeSpan.FromMinutes(5);
                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
