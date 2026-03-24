namespace UrGuide.WebApp.Observability;

/// <summary>
/// Records security-relevant events to the application log.
/// All entries are written at the Warning or Error level and include
/// the authenticated user ID (or "anonymous") and the remote IP address
/// so that they can be queried and alerted on in log-aggregation systems.
/// </summary>
public interface ISecurityEventLogger
{
    /// <summary>Logs a successful authentication.</summary>
    void LogLoginSuccess(string userId, string ipAddress);

    /// <summary>Logs a failed authentication attempt.</summary>
    void LogLoginFailure(string username, string ipAddress, string reason);

    /// <summary>Logs an account lockout.</summary>
    void LogAccountLockout(string userId, string ipAddress);

    /// <summary>Logs a successful password change or reset.</summary>
    void LogPasswordChanged(string userId, string ipAddress);

    /// <summary>Logs an authorisation failure (403).</summary>
    void LogUnauthorizedAccess(string userId, string resource, string ipAddress);

    /// <summary>Logs a suspicious or potentially malicious request.</summary>
    void LogSuspiciousActivity(string userId, string ipAddress, string details);

    /// <summary>Logs a two-factor authentication event.</summary>
    void Log2FAEvent(string userId, string eventType, bool success);
}
