using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using NLog;
using NLog.Web;
using UrGuide.WebApp.Data;
using UrGuide.Services.Extensions;
using UrGuide.WebApp.Extensions;
using Microsoft.EntityFrameworkCore;
using AspNetCoreRateLimit;
using Microsoft.OpenApi;
using UrGuide.WebApp.Hubs;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics;
using UrGuide.WebApp.Models;
using Duende.IdentityServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Asp.Versioning;
using System.Linq;
using UrGuide.ServiceDefaults;
using UrGuide.WebApp.MessageQueue;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
try
{
    logger.Debug("init main");
    
    var builder = WebApplication.CreateBuilder(args);

    // Configure logging first before adding service defaults
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    // Add .NET Aspire service defaults (OpenTelemetry, health checks, service discovery, resilience)
    // This must come after ClearProviders to ensure OpenTelemetry logging is not removed
    builder.AddServiceDefaults();

    // Add services to the container.
    builder.Services.AddUrGuideAuthServices(builder.Configuration);
    builder.Services.AddUrGuideServices(builder.Configuration);

    // Add HttpClient for calling IdentityServer token endpoint
    builder.Services.AddHttpClient();

    // Configure rate limiting
    builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
    builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
    builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
    builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

    // Configure tiered rate limiting
    builder.Services.Configure<UrGuide.WebApp.RateLimiting.RateLimitOptions>(builder.Configuration.GetSection("TieredRateLimit"));
    builder.Services.AddSingleton<UrGuide.WebApp.RateLimiting.IRateLimitAnalyticsService, UrGuide.WebApp.RateLimiting.RateLimitAnalyticsService>();
    builder.Services.AddMemoryCache(); // Required for rate limiting cache

    builder.Services.AddControllersWithViews(options => {
        options.Filters.Add(typeof(UrGuide.WebApp.Filters.EnvelopResultFilter));
        options.Filters.Add(typeof(UrGuide.WebApp.Filters.SaveChangeActionFilter));
    });
    
    // FluentValidation is automatically configured via the UrGuide.Services registration
    
    // Add API versioning
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"),
            new QueryStringApiVersionReader("api-version")
        );
    }).AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
    
    // Add message queue
    builder.Services.AddMessageQueue(builder.Configuration);
    
    // Add health checks
    var authConn = builder.Configuration.GetSection("ConnectionStrings:AuthConnection").Value;
    var dataConn = builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
    var elasticsearchUrl = builder.Configuration.GetSection("Elasticsearch:Url").Value ?? "http://localhost:9200";
    builder.Services.AddHealthChecks()
        .AddSqlServer(authConn ?? "", name: "auth-db")
        .AddSqlServer(dataConn ?? "", name: "data-db")
        .AddMessageQueueHealthChecks(builder.Configuration)
        .AddElasticsearch(elasticsearchUrl, name: "elasticsearch");
    
    // Add CORS policy for API consumers
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ApiCorsPolicy", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("X-Api-Version");
        });
    });
    
    // Add response caching
    builder.Services.AddResponseCaching();
    builder.Services.AddOutputCache(options =>
    {
        options.AddBasePolicy(policy => policy.Expire(TimeSpan.FromMinutes(10)));
    });
    
    builder.Services.AddSwaggerGen(c =>
    {
        // OAuth2 configuration for Authorization Code flow
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
                        { IdentityServerConstants.StandardScopes.OpenId, "OpenID Connect" },
                        { IdentityServerConstants.StandardScopes.Profile, "User Profile" },
                        { IdentityServerConstants.StandardScopes.OfflineAccess, "Offline Access" }
                    }
                }
            }
        });

        // JWT Bearer token configuration - allows pasting token directly
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description = "Paste your JWT token here. You can get a token from POST /api/auth/token endpoint.",
            In = ParameterLocation.Header
        });

        c.OperationFilter<UrGuide.WebApp.Filters.AuthorizeCheckOperationFilter>();
        
        // Configure Swagger to use API versioning
        c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        c.SwaggerDoc("v1", new OpenApiInfo 
        { 
            Title = "UrGuide API", 
            Version = "v1",
            Description = "UrGuide Tourism Platform API - Connect travelers with local guides",
            Contact = new OpenApiContact
            {
                Name = "UrGuide Support",
                Url = new Uri("https://github.com/JeanMarcMbouma/urguide")
            }
        });
    });

    builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    
    // Add background services
    builder.Services.AddHostedService<UrGuide.WebApp.Services.DataExportBackgroundService>();

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
                    errors.AddRange([exceptionHandler.Error.Message, exceptionHandler.Error.StackTrace ?? ""]);
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
        
        // Seed admin user (if enabled in Development)
        if (app.Environment.IsDevelopment())
        {
            var adminSeedingService = scope.ServiceProvider.GetRequiredService<UrGuide.WebApp.Services.IAdminSeedingService>();
            await adminSeedingService.SeedDefaultAdminAsync();
        }
    }

    // Disable legacy IP rate limiting to avoid conflicts with tiered rate limiting
    // app.UseIpRateLimiting();

    app.UseForwardedHeaders(new ForwardedHeadersOptions { 
        ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
    });

    // Add CORS middleware
    app.UseCors("ApiCorsPolicy");
    
    // Add caching middleware
    app.UseResponseCaching();
    app.UseOutputCache();

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
    
    // Add tiered rate limiting middleware after authentication and routing, before authorization
    app.UseMiddleware<UrGuide.WebApp.RateLimiting.TieredRateLimitMiddleware>();
    
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    app.MapHub<NotificationHub>("/notify");
    
    // Map Aspire default endpoints (health checks at /health and /alive)
    app.MapDefaultEndpoints();

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
