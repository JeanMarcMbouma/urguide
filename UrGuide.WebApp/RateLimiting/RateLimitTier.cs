namespace UrGuide.WebApp.RateLimiting
{
    /// <summary>
    /// Defines the different tiers for rate limiting
    /// </summary>
    public enum RateLimitTier
    {
        /// <summary>
        /// Anonymous users (not authenticated)
        /// </summary>
        Anonymous = 0,

        /// <summary>
        /// Authenticated users with basic subscription
        /// </summary>
        Authenticated = 1,

        /// <summary>
        /// Premium users with premium subscription
        /// </summary>
        Premium = 2,

        /// <summary>
        /// Internal services (exempt from rate limiting)
        /// </summary>
        Internal = 99
    }
}
