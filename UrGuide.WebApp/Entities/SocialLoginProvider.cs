using System;

namespace UrGuide.WebApp.Entities
{
    /// <summary>
    /// Tracks social login providers linked to a user account.
    /// </summary>
    public class SocialLoginProvider
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public UrGuideUser User { get; set; } = null!;

        /// <summary>
        /// Provider name (e.g., "Google", "Apple", "Microsoft").
        /// </summary>
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Provider-specific unique user identifier.
        /// </summary>
        public string ProviderKey { get; set; } = string.Empty;

        /// <summary>
        /// Email returned by the provider (may be a relay address for Apple).
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Display name from the provider profile.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Avatar URL from the provider profile.
        /// </summary>
        public string? AvatarUrl { get; set; }

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}
