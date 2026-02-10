using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Analytics;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IAnalyticsService
    {
        Task<Result<UserRegistrationTrends>> GetUserRegistrationTrendsAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Result<TourBookingStatistics>> GetTourBookingStatisticsAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Result<RevenueMetrics>> GetRevenueMetricsAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Result<GuidePerformanceMetrics>> GetGuidePerformanceMetricsAsync(AnalyticsDateRange dateRange, int topN, CancellationToken cancellationToken);
        Task<Result<PopularDestinations>> GetPopularDestinationsAsync(AnalyticsDateRange dateRange, int topN, CancellationToken cancellationToken);
        Task<Result<ConversionFunnel>> GetConversionFunnelAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Result<DashboardSummary>> GetDashboardSummaryAsync(AnalyticsDateRange dateRange, CancellationToken cancellationToken);
        Task<Result<byte[]>> ExportDashboardDataAsync(AnalyticsDateRange dateRange, string format, CancellationToken cancellationToken);
    }
}
