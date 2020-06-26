using Microsoft.AspNetCore.Http;
using System;
using UrGuide.Shared.Contracts;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Net;
using System.Security.Claims;
using System.Linq;

namespace UrGuide.WebApp.Services
{
    public class UserContext : IUserContext
    {
        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public IHttpContextAccessor HttpContextAccessor { get; }
#if DEBUG
        public string UserId => HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "00000000-0000-0000-0000-000000000000";
#else
        public string UserId => HttpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
#endif

        public Task<string> Id_Token => HttpContextAccessor.HttpContext.GetTokenAsync("id_token");
        public Task<string> Access_Token => HttpContextAccessor.HttpContext.GetTokenAsync("access_token");
        public bool IsAuthenticated => HttpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
#if DEBUG
        public IPAddress IPAddress => IPAddress.Parse("176.67.20.135");
#else
        public IPAddress IPAddress
        {
            get
            {
                var result = string.Empty;
                try
                {
                    //first try to get IP address from the forwarded header
                    if (HttpContextAccessor.HttpContext.Request.Headers != null)
                    {
                        //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                        //connecting to a web server through an HTTP proxy or load balancer
                        var forwardedHeader = HttpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
                        if (!string.IsNullOrEmpty(forwardedHeader))
                            result = forwardedHeader.FirstOrDefault();
                    }

                    //if this header not exists try get connection remote IP address
                    if (string.IsNullOrEmpty(result) && HttpContextAccessor.HttpContext.Connection.RemoteIpAddress != null)
                        result = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                catch
                {
                    return IPAddress.Loopback;
                }

                //some of the validation
                if (result != null && result.Equals(IPAddress.IPv6Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    result = IPAddress.Loopback.ToString();

                //"TryParse" doesn't support IPv4 with port number
                if (IPAddress.TryParse(result ?? string.Empty, out var ip))
                    //IP address is valid 
                    result = ip.ToString();
                else if (!string.IsNullOrEmpty(result))
                    //remove port
                    result = result.Split(':').FirstOrDefault();

                return IPAddress.Parse(result);
            }
        }
#endif
    }
}
