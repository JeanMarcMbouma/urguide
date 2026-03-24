using Microsoft.Extensions.Logging;

namespace UrGuide.WebApp.Observability;

/// <summary>
/// Default implementation of <see cref="IUserActivityLogger"/>.
/// Uses a dedicated <see cref="ILogger"/> category (<c>UserActivity</c>) so that
/// NLog rules can route activity entries to separate targets (e.g. Seq, Elasticsearch)
/// independently of general application logs.
/// </summary>
public sealed class UserActivityLogger : IUserActivityLogger
{
    private readonly ILogger<UserActivityLogger> _logger;

    public UserActivityLogger(ILogger<UserActivityLogger> logger)
    {
        _logger = logger;
    }

    public void LogProfileActivity(string userId, string action)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} performed profile action {Action}",
            userId, action);

    public void LogTourActivity(string userId, string action, string tourId)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} performed tour action {Action} on tour {TourId}",
            userId, action, tourId);

    public void LogPaymentActivity(string userId, string action, string paymentId)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} performed payment action {Action} for payment {PaymentId}",
            userId, action, paymentId);

    public void LogSearchActivity(string userId, string query, int resultCount)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} searched for {Query}, got {ResultCount} results",
            userId, query, resultCount);

    public void LogReviewActivity(string userId, string action, string targetId)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} performed review action {Action} on target {TargetId}",
            userId, action, targetId);

    public void LogMessagingActivity(string userId, string action, string conversationId)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} performed messaging action {Action} in conversation {ConversationId}",
            userId, action, conversationId);

    public void LogCustomActivity(string userId, string category, string action, string? details = null)
        => _logger.LogInformation(
            "ACTIVITY: User {UserId} performed {Category}/{Action}. Details: {Details}",
            userId, category, action, details ?? "N/A");
}
