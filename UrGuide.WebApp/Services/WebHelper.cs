using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System;
using UrGuide.Shared;
using UrGuide.Shared.Contracts;

namespace UrGuide.WebApp.Services
{
    public class WebHelper : IWebHelper
    {
        public WebHelper(IHttpContextAccessor httpContext)
        {
            HC = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }
        
        public IHttpContextAccessor HC { get; }

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
