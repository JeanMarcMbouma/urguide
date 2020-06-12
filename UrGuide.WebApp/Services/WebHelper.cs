using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using UrGuide.Shared;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Services
{
    public class WebHelper : IWebHelper
    {
        private readonly IWebHostEnvironment _environment;

        public WebHelper(IHttpContextAccessor httpContext, IWebHostEnvironment environment, IOptions<SpaStaticFilesOptions> spaOptions)
        {
            HC = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            SpaOptions = spaOptions ?? throw new ArgumentNullException(nameof(spaOptions));
        }
        
        public IHttpContextAccessor HC { get; }
        public IOptions<SpaStaticFilesOptions> SpaOptions { get; }

        public string ImageDirectoryPath => Path.Combine(_environment.ContentRootPath,
#if !DEBUG
            SpaOptions.Value.RootPath
#else
            SpaOptions.Value.RootPath.Replace("build", "public")
#endif
            , "images");

        public string ThumbImageDirectoryPath => Path.Combine(_environment.ContentRootPath,
#if !DEBUG
            SpaOptions.Value.RootPath
#else
            SpaOptions.Value.RootPath.Replace("build", "public")
#endif
            , "thumb");

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
            var request = HC.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            foreach (var key in p.Keys)
            {
                if (p[key] == null) continue;
                uri = QueryHelpers.AddQueryString(uri, key, p[key].ToString());
            }
            return $"{baseUrl}{uri}";
        }
    }
}
