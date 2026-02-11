using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.IO;
using UrGuide.Shared;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Services
{
    public class WebHelper : IWebHelper
    {
        private readonly IWebHostEnvironment _environment;

        public WebHelper(IHttpContextAccessor httpContext, IWebHostEnvironment environment)
        {
            HC = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }
        
        public IHttpContextAccessor HC { get; }

        public string ImageDirectoryPath => Path.Combine(_environment.ContentRootPath, "images");

        public string ThumbImageDirectoryPath => Path.Combine(_environment.ContentRootPath, "thumb");

        public string ResolveImageUrl(string imageFileName)
        {
            return $"images/{imageFileName}";
        }

        public string ResolveUrl(MessageTypes confirmation, object values)
        {
            return confirmation switch
            {
                MessageTypes.Confirmation => ResolveUrl("/account/confirmemail", values),
                MessageTypes.PasswordReset => ResolveUrl("/pforget", values),
                _ => ResolveUrl("", values),
            };
        }

        public string ResolveUrl(string uri, object values)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            var p = new RouteValueDictionary(values);
            var httpContext = HC.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");
            var request = httpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            foreach (var key in p.Keys)
            {
                var value = p[key]?.ToString();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                uri = QueryHelpers.AddQueryString(uri, key, value);
            }
            return $"{baseUrl}{uri}";
        }
    }
}
