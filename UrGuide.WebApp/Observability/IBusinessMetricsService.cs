namespace UrGuide.WebApp.Observability;

/// <summary>
/// Records custom business-level metrics for key domain events.
/// Backed by OpenTelemetry System.Diagnostics.Metrics so measurements are
/// automatically exported to any configured exporter (OTLP, Prometheus, etc.).
/// </summary>
public interface IBusinessMetricsService
{
    /// <summary>Records a new user registration.</summary>
    void RecordUserRegistration();

    /// <summary>Records a user login event.</summary>
    void RecordUserLogin();

    /// <summary>Records a tour post creation.</summary>
    void RecordTourCreated();

    /// <summary>Records a tour booking (seat reservation).</summary>
    void RecordTourBooked();

    /// <summary>Records a payment attempt. <paramref name="succeeded"/> indicates the outcome.</summary>
    void RecordPayment(bool succeeded, decimal amount, string currency);

    /// <summary>Records a guide payout.</summary>
    void RecordPayout(decimal amount, string currency);

    /// <summary>Records a review/feedback submission.</summary>
    void RecordReviewSubmitted();

    /// <summary>Records a search query.</summary>
    void RecordSearch();

    /// <summary>Records an API rate-limit hit.</summary>
    void RecordRateLimitHit(string tier);
}
