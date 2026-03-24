using Microsoft.Extensions.Logging;

namespace UrGuide.WebApp.Observability;

/// <summary>
/// Default implementation of <see cref="ISecurityEventLogger"/>.
/// Uses a named logger with category <c>Security</c> (created via <see cref="ILoggerFactory"/>)
/// so that NLog rules can route security events to a separate target or log-aggregation
/// pipeline independent of general application logs.
/// All events are logged at Warning or higher so they are captured by aggregation
/// targets (e.g. Seq) that are configured for Warning+.
/// </summary>
public sealed class SecurityEventLogger : ISecurityEventLogger
{
    private readonly ILogger _logger;

    public SecurityEventLogger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("Security");
    }

    public void LogLoginSuccess(string userId, string ipAddress)
        => _logger.LogWarning(
            "SECURITY: Login succeeded for user {UserId} from {IpAddress}",
            userId, ipAddress);

    public void LogLoginFailure(string username, string ipAddress, string reason)
        => _logger.LogWarning(
            "SECURITY: Login failed for username {Username} from {IpAddress}. Reason: {Reason}",
            username, ipAddress, reason);

    public void LogAccountLockout(string userId, string ipAddress)
        => _logger.LogWarning(
            "SECURITY: Account locked out for user {UserId} from {IpAddress}",
            userId, ipAddress);

    public void LogPasswordChanged(string userId, string ipAddress)
        => _logger.LogWarning(
            "SECURITY: Password changed for user {UserId} from {IpAddress}",
            userId, ipAddress);

    public void LogUnauthorizedAccess(string userId, string resource, string ipAddress)
        => _logger.LogWarning(
            "SECURITY: Unauthorized access attempt by user {UserId} to resource {Resource} from {IpAddress}",
            userId, resource, ipAddress);

    public void LogSuspiciousActivity(string userId, string ipAddress, string details)
        => _logger.LogError(
            "SECURITY: Suspicious activity detected for user {UserId} from {IpAddress}. Details: {Details}",
            userId, ipAddress, details);

    public void Log2FAEvent(string userId, string eventType, bool success)
        => _logger.LogWarning(
            "SECURITY: 2FA event {EventType} for user {UserId}. Success: {Success}",
            eventType, userId, success);
}
