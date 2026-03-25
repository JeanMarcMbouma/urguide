using System;

namespace UrGuide.WebApp.Entities
{
    /// <summary>
    /// Audit log entry for social login events (link, unlink, login, conflict).
    /// </summary>
    public class SocialLoginAuditLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Action performed: "Linked", "Unlinked", "Login", "ConflictResolved", "AccountCreated".
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Additional details about the action (e.g., conflict email, merge info).
        /// </summary>
        public string? Details { get; set; }

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
