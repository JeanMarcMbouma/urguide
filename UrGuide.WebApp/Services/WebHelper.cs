using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public string ImageDirectoryPath => Path.Combine(_environment.ContentRootPath, "ClientApp/public/images");

        public string ThumbImageDirectoryPath => Path.Combine(_environment.ContentRootPath, "ClientApp/public/thumb");

        public string ResolveImageThumbUrl(string imageSeoName)
        {
            return ResolveUrl($"/thumb/{imageSeoName}", new { });
        }

        public string ResolveImageUrl(string imageSeoName)
        {
            return ResolveUrl($"/images/{imageSeoName}", new { });
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
            var request = HC.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            foreach (var key in p.Keys)
            {
                uri = QueryHelpers.AddQueryString(uri, key, p[key].ToString());
            }
            return $"{baseUrl}{uri}";
        }
    }
}
