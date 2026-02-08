using System;

namespace UrGuide.WebApp.Attributes
{
    /// <summary>
    /// Attribute to exempt an endpoint from rate limiting
    /// Useful for internal services or health check endpoints
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RateLimitExemptAttribute : Attribute
    {
        /// <summary>
        /// Reason for the exemption (for documentation purposes)
        /// </summary>
        public string Reason { get; set; }

        public RateLimitExemptAttribute(string reason = "Internal service")
        {
            Reason = reason;
        }
    }
}
