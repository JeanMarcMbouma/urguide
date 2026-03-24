using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace UrGuide.WebApp.Observability;

/// <summary>
/// OpenTelemetry-backed implementation of <see cref="IBusinessMetricsService"/>.
/// A single <see cref="Meter"/> named <c>UrGuide.Business</c> owns all instruments.
/// The meter is registered in the OpenTelemetry pipeline via
/// <c>AddMeter("UrGuide.Business")</c> in <c>ServiceDefaults/Extensions.cs</c>.
/// </summary>
public sealed class BusinessMetricsService : IBusinessMetricsService, IDisposable
{
    public const string MeterName = "UrGuide.Business";

    private readonly Meter _meter;

    // Counters
    private readonly Counter<long> _userRegistrations;
    private readonly Counter<long> _userLogins;
    private readonly Counter<long> _toursCreated;
    private readonly Counter<long> _toursBooked;
    private readonly Counter<long> _paymentsAttempted;
    private readonly Counter<long> _paymentsSucceeded;
    private readonly Counter<long> _reviewsSubmitted;
    private readonly Counter<long> _searchQueries;
    private readonly Counter<long> _rateLimitHits;

    // Histograms / value recorders
    private readonly Histogram<double> _paymentAmount;
    private readonly Histogram<double> _payoutAmount;

    public BusinessMetricsService()
    {
        _meter = new Meter(MeterName, "1.0");

        _userRegistrations  = _meter.CreateCounter<long>("urguide.users.registrations",   "registrations", "Total user registrations");
        _userLogins         = _meter.CreateCounter<long>("urguide.users.logins",           "logins",        "Total user login events");
        _toursCreated       = _meter.CreateCounter<long>("urguide.tours.created",          "tours",         "Total tour posts created");
        _toursBooked        = _meter.CreateCounter<long>("urguide.tours.booked",           "bookings",      "Total tour seat reservations");
        _paymentsAttempted  = _meter.CreateCounter<long>("urguide.payments.attempted",     "payments",      "Total payment attempts");
        _paymentsSucceeded  = _meter.CreateCounter<long>("urguide.payments.succeeded",     "payments",      "Total successful payments");
        _reviewsSubmitted   = _meter.CreateCounter<long>("urguide.reviews.submitted",      "reviews",       "Total reviews submitted");
        _searchQueries      = _meter.CreateCounter<long>("urguide.search.queries",         "queries",       "Total search queries");
        _rateLimitHits      = _meter.CreateCounter<long>("urguide.ratelimit.hits",         "hits",          "Total rate-limit rejections");

        _paymentAmount = _meter.CreateHistogram<double>("urguide.payments.amount", "currency_units", "Payment amounts");
        _payoutAmount  = _meter.CreateHistogram<double>("urguide.payouts.amount",  "currency_units", "Payout amounts");
    }

    public void RecordUserRegistration()   => _userRegistrations.Add(1);
    public void RecordUserLogin()          => _userLogins.Add(1);
    public void RecordTourCreated()        => _toursCreated.Add(1);
    public void RecordTourBooked()         => _toursBooked.Add(1);
    public void RecordReviewSubmitted()    => _reviewsSubmitted.Add(1);
    public void RecordSearch()             => _searchQueries.Add(1);

    public void RecordPayment(bool succeeded, decimal amount, string currency)
    {
        _paymentsAttempted.Add(1, new KeyValuePair<string, object?>("currency", currency));
        if (succeeded)
        {
            _paymentsSucceeded.Add(1, new KeyValuePair<string, object?>("currency", currency));
            _paymentAmount.Record((double)amount, new KeyValuePair<string, object?>("currency", currency));
        }
    }

    public void RecordPayout(decimal amount, string currency)
        => _payoutAmount.Record((double)amount, new KeyValuePair<string, object?>("currency", currency));

    public void RecordRateLimitHit(string tier)
        => _rateLimitHits.Add(1, new KeyValuePair<string, object?>("tier", tier));

    public void Dispose() => _meter.Dispose();
}
