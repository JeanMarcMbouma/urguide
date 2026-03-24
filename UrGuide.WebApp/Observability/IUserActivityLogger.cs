namespace UrGuide.WebApp.Observability;

/// <summary>
/// Records user-activity events to the application log.
/// Provides a single, consistent interface for logging what users do,
/// keeping activity-tracking logic out of individual controllers/services.
/// </summary>
public interface IUserActivityLogger
{
    /// <summary>Logs a profile view or update.</summary>
    void LogProfileActivity(string userId, string action);

    /// <summary>Logs a tour-related activity (create, book, cancel, etc.).</summary>
    void LogTourActivity(string userId, string action, string tourId);

    /// <summary>Logs a payment activity.</summary>
    void LogPaymentActivity(string userId, string action, string paymentId);

    /// <summary>Logs a search query.</summary>
    void LogSearchActivity(string userId, string query, int resultCount);

    /// <summary>Logs a review or feedback submission.</summary>
    void LogReviewActivity(string userId, string action, string targetId);

    /// <summary>Logs a messaging event (send, read, etc.).</summary>
    void LogMessagingActivity(string userId, string action, string conversationId);

    /// <summary>Logs an arbitrary custom activity event.</summary>
    void LogCustomActivity(string userId, string category, string action, string? details = null);
}
