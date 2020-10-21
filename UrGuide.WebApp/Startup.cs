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
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Hubs;
using IdentityServer4;
using Microsoft.Extensions.FileProviders;
using System.IO;

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
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("/connect/authorize", UriKind.Relative),
                            TokenUrl = new Uri("/connect/token", UriKind.Relative),
                            Scopes = new Dictionary<string, string>
                            {
                                { IdentityServerConstants.StandardScopes.OpenId, "UrGuide Web Application" },
                                { IdentityServerConstants.StandardScopes.Profile, "UrGuide Web Application" },
                                { IdentityServerConstants.StandardScopes.OfflineAccess, "UrGuide Web Application" }
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

            app.UseExceptionHandler(options =>
            {
                options.Run(async (request) =>
                {
                    var exceptionHandler = request.Features.Get<IExceptionHandlerFeature>();
                    if(exceptionHandler != null)
                    {
                        request.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                        var errors = new List<string>();
                        if(env.IsDevelopment())
                        {
                            errors.AddRange(new[] { exceptionHandler.Error.Message, exceptionHandler.Error.StackTrace });
                        } else
                        {
                            errors.Add("An unexpected error has occured.");
                        }
                        var result = ErrorEnvelop.Create(errors);
                        await request.Response.WriteJsonAsync(result);
                    }
                });
            });

            if (!env.IsDevelopment())
            { 
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            serviceProvider.GetRequiredService<UrGuideAuthContext>().Database.Migrate();
            serviceProvider.GetRequiredService<UrGuide.Data.UrGuideContext>().Database.Migrate();

            app.UseIpRateLimiting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions { 
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

          //  app.UseStaticFiles(new StaticFileOptions
          //  {
          //      FileProvider = new PhysicalFileProvider(
          //Path.Combine(Directory.GetCurrentDirectory(), "ClientApp/src")),
          //      RequestPath = "/ClientApp/src",

          //  });
          //  app.UseDirectoryBrowser(new DirectoryBrowserOptions
          //  {
          //      FileProvider = new PhysicalFileProvider(
          //                  Path.Combine(Directory.GetCurrentDirectory(), "ClientApp/src")),
          //      RequestPath = "/ClientApp/src"
          //  });
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ur Guide API v1");
                c.OAuthClientId("UrGuide.WebAPI");
                c.OAuthAppName("UrGuide Swagger UI");
                c.OAuthUsePkce();
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
                endpoints.MapHub<NotificationHub>("/notify");
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
