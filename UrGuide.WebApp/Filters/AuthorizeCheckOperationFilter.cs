using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace UrGuide.WebApp.Filters
{
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for authorize attribute
            var declaringType = context.MethodInfo.DeclaringType;
            var hasAuthorize = ((declaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ?? false) ||
                               context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any())
                               && !context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any();

            if (!hasAuthorize) return;

            operation.Responses?.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses?.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecuritySchemeReference("oauth2", context.Document);

            operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        [ oAuthScheme ] = new List<string> { "openid", "profile", "offline_access" }
                    }
                };
        }
    }
}
