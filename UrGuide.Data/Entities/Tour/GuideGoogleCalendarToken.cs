using System;

namespace UrGuide.Data.Entities.Tour
{
    /// <summary>
    /// Stores a guide's Google Calendar OAuth 2.0 tokens, encrypted at rest using
    /// ASP.NET Core Data Protection.
    /// </summary>
    public class GuideGoogleCalendarToken
    {
        public string Id { get; set; } = null!;
        public string GuideId { get; set; } = null!;
        public virtual Users.User Guide { get; set; } = null!;

        /// <summary>Data-Protection-encrypted access token.</summary>
        public string EncryptedAccessToken { get; set; } = null!;

        /// <summary>Data-Protection-encrypted refresh token (null for implicit grants).</summary>
        public string? EncryptedRefreshToken { get; set; }

        /// <summary>Token type returned by Google (typically "Bearer").</summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>OAuth scopes granted by the user.</summary>
        public string Scope { get; set; } = string.Empty;

        /// <summary>UTC time at which the access token expires.</summary>
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
