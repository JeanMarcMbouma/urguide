using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.WebApp.Entities;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// Manages social login provider operations: login, link, unlink, and audit logging.
    /// </summary>
    public interface ISocialAuthService
    {
        /// <summary>
        /// Processes a social login callback. Creates a new user or links to an existing account.
        /// Returns the user and a JWT token.
        /// </summary>
        Task<SocialAuthResult> ProcessSocialLoginAsync(
            string provider,
            string providerKey,
            string? email,
            string? firstName,
            string? lastName,
            string? avatarUrl,
            CancellationToken cancellationToken);

        /// <summary>
        /// Links a social provider to an existing authenticated user account.
        /// </summary>
        Task<SocialAuthResult> LinkProviderAsync(
            string userId,
            string provider,
            string providerKey,
            string? email,
            string? displayName,
            string? avatarUrl,
            CancellationToken cancellationToken);

        /// <summary>
        /// Unlinks a social provider from an authenticated user account.
        /// </summary>
        Task<SocialAuthResult> UnlinkProviderAsync(
            string userId,
            string provider,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets all linked social providers for a user.
        /// </summary>
        Task<IReadOnlyList<SocialLoginProviderDto>> GetLinkedProvidersAsync(
            string userId,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets audit log entries for a user's social login activities.
        /// </summary>
        Task<IReadOnlyList<SocialLoginAuditLogDto>> GetAuditLogAsync(
            string userId,
            int take,
            CancellationToken cancellationToken);

        /// <summary>
        /// Logs a social login audit event.
        /// </summary>
        Task LogAuditEventAsync(
            string userId,
            string provider,
            string action,
            string? details,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken);
    }

    public class SocialAuthResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? UserId { get; set; }
        public string? Token { get; set; }
        public bool IsNewAccount { get; set; }

        public static SocialAuthResult Ok(string userId, string? token = null, bool isNew = false)
            => new() { Success = true, UserId = userId, Token = token, IsNewAccount = isNew };

        public static SocialAuthResult Fail(string error)
            => new() { Success = false, Error = error };
    }

    public class SocialLoginProviderDto
    {
        public string Provider { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime LinkedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class SocialLoginAuditLogDto
    {
        public string Provider { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
