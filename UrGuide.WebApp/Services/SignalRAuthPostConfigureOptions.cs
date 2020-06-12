using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Services
{
    public class SignalRAuthPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
    {
        public void PostConfigure(string name, JwtBearerOptions options)
        {
            options.Events.OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/notify")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            };
        }
    }
}
