using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using NLog.Web;
using UrGuide.WebApp.Data;
using UrGuide.Services.Extensions;
using UrGuide.WebApp.Extensions;
using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;
using UrGuide.WebApp.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics;
using UrGuide.WebApp.Models;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
try
{
    logger.Debug("init main");
    
    var builder = WebApplication.CreateBuilder(args);

    // Configure logging
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddUrGuideAuthServices(builder.Configuration);
    builder.Services.AddUrGuideServices(builder.Configuration);

    // Configure rate limiting
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

    builder.Services.AddControllersWithViews(options => {
        options.Filters.Add(typeof(UrGuide.WebApp.Filters.EnvelopResultFilter));
        options.Filters.Add(typeof(UrGuide.WebApp.Filters.SaveChangeActionFilter));
    });
    
    // FluentValidation is automatically configured via the UrGuide.Services registration
    
    builder.Services.AddSwaggerGen(c =>
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
        c.OperationFilter<UrGuide.WebApp.Filters.AuthorizeCheckOperationFilter>();
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "UrGuide API", Version = "v1" });
    });

    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.UseExceptionHandler(options =>
    {
        options.Run(async (request) =>
        {
            var exceptionHandler = request.Features.Get<IExceptionHandlerFeature>();
            if(exceptionHandler != null)
            {
                request.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
                var errors = new List<string>();
                if(app.Environment.IsDevelopment())
                {
                    errors.AddRange(new[] { exceptionHandler.Error.Message, exceptionHandler.Error.StackTrace });
                } else
                {
                    errors.Add("An unexpected error has occured.");
                }
                var result = ErrorEnvelop.Create(errors);
                await request.Response.WriteAsJsonAsync(result);
            }
        });
    });

    if (!app.Environment.IsDevelopment())
    { 
        app.UseHsts();
        app.UseHttpsRedirection();
    }

    // Database migration and seeding
    using (var scope = app.Services.CreateScope())
    {
        var authContext = scope.ServiceProvider.GetRequiredService<UrGuideAuthContext>();
        var dataContext = scope.ServiceProvider.GetRequiredService<UrGuide.Data.UrGuideContext>();
        
        authContext.Database.Migrate();
        dataContext.Database.Migrate();

        // Seed initial data
        var seedingService = scope.ServiceProvider.GetRequiredService<UrGuide.Services.Contracts.IDataSeedingService>();
        await seedingService.SeedDataAsync();
    }

    app.UseIpRateLimiting();

    app.UseForwardedHeaders(new ForwardedHeadersOptions { 
        ForwardedHeaders = ForwardedHeaders.XForwardedProto
    });

    app.UseRouting();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrGuide API v1");
        c.OAuthClientId("UrGuide.WebAPI");
        c.OAuthAppName("UrGuide Swagger UI");
        c.OAuthUsePkce();
    });
    app.UseAuthentication();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    app.MapHub<NotificationHub>("/notify");

    app.Run();
}
catch (Exception exception)
{
    //NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
