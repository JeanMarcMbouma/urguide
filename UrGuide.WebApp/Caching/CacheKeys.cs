namespace UrGuide.WebApp.Caching
{
    /// <summary>
    /// Well-known cache key prefixes / templates used across the application.
    /// Using constants prevents typos and makes invalidation explicit.
    /// </summary>
    public static class CacheKeys
    {
        // ── Tours ──────────────────────────────────────────────────────────────
        public const string TourPrefix        = "tour";
        public static string Tour(string id)  => $"{TourPrefix}:{id}";
        public static string TourList()       => $"{TourPrefix}:list";

        // ── Users ──────────────────────────────────────────────────────────────
        public const string UserPrefix        = "user";
        public static string UserProfile(string userId) => $"{UserPrefix}:profile:{userId}";

        // ── Recommendations ────────────────────────────────────────────────────
        public const string RecommendationPrefix               = "recommendation";
        public static string Recommendations(string userId)    => $"{RecommendationPrefix}:{userId}";

        // ── Lookup / catalog ───────────────────────────────────────────────────
        public const string LookupPrefix      = "lookup";
        public static string Lookup(string kind) => $"{LookupPrefix}:{kind}";

        // ── Rate limiting ──────────────────────────────────────────────────────
        public const string RateLimitPrefix   = "ratelimit";
        public static string RateLimit(string tier, string identifier, string endpoint) =>
            $"{RateLimitPrefix}:{tier}:{identifier}:{endpoint}";
        public static string RateLimitReset(string tier, string identifier, string endpoint) =>
            $"{RateLimitPrefix}:{tier}:{identifier}:{endpoint}:reset";
    }
}
