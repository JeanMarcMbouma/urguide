using System.Threading;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Model.Analytics;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IAnalyticsService
    {
        Task<Outcome<UserRegistrationTrends>> GetUserRegistrationTrendsAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Outcome<TourBookingStatistics>> GetTourBookingStatisticsAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Outcome<RevenueMetrics>> GetRevenueMetricsAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Outcome<GuidePerformanceMetrics>> GetGuidePerformanceMetricsAsync(AnalyticsDateRange dateRange, int topN, CancellationToken cancellationToken);
        Task<Outcome<PopularDestinations>> GetPopularDestinationsAsync(AnalyticsDateRange dateRange, int topN, CancellationToken cancellationToken);
        Task<Outcome<ConversionFunnel>> GetConversionFunnelAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Outcome<DashboardSummary>> GetDashboardSummaryAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Outcome<byte[]>> ExportDashboardDataAsync(AnalyticsDateRange dateRange, string format, CancellationToken cancellationToken);
    }
}
