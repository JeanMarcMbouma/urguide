using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Analytics;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;
using BbQ.Outcome;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Controller for analytics dashboard and metrics (Admin only)
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        /// <summary>
        /// Get user registration trends
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User registration trends data</returns>
        [HttpGet("user-registration-trends")]
        [ProducesResponseType(200, Type = typeof(UserRegistrationTrends))]
        public async Task<IActionResult> GetUserRegistrationTrends(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetUserRegistrationTrendsAsync(dateRange, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get tour booking statistics
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Tour booking statistics</returns>
        [HttpGet("tour-booking-statistics")]
        [ProducesResponseType(200, Type = typeof(TourBookingStatistics))]
        public async Task<IActionResult> GetTourBookingStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetTourBookingStatisticsAsync(dateRange, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get revenue metrics
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Revenue metrics</returns>
        [HttpGet("revenue-metrics")]
        [ProducesResponseType(200, Type = typeof(RevenueMetrics))]
        public async Task<IActionResult> GetRevenueMetrics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetRevenueMetricsAsync(dateRange, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get guide performance metrics
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="topN">Number of top performers to return (default: 10)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Guide performance metrics</returns>
        [HttpGet("guide-performance")]
        [ProducesResponseType(200, Type = typeof(GuidePerformanceMetrics))]
        public async Task<IActionResult> GetGuidePerformance(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            [FromQuery] int topN = 10,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetGuidePerformanceMetricsAsync(dateRange, topN, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get popular destinations
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="topN">Number of top destinations to return (default: 10)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Popular destinations</returns>
        [HttpGet("popular-destinations")]
        [ProducesResponseType(200, Type = typeof(PopularDestinations))]
        public async Task<IActionResult> GetPopularDestinations(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            [FromQuery] int topN = 10,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetPopularDestinationsAsync(dateRange, topN, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get conversion funnel analytics
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Conversion funnel data</returns>
        [HttpGet("conversion-funnel")]
        [ProducesResponseType(200, Type = typeof(ConversionFunnel))]
        public async Task<IActionResult> GetConversionFunnel(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetConversionFunnelAsync(dateRange, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get complete dashboard summary
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Complete dashboard summary with all metrics</returns>
        [HttpGet("dashboard")]
        [ProducesResponseType(200, Type = typeof(DashboardSummary))]
        public async Task<IActionResult> GetDashboardSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.GetDashboardSummaryAsync(dateRange, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Export dashboard data
        /// </summary>
        /// <param name="startDate">Start date for analytics (optional, defaults to 6 months ago)</param>
        /// <param name="endDate">End date for analytics (optional, defaults to now)</param>
        /// <param name="period">Period grouping (Hourly, Daily, Weekly, Monthly, Yearly)</param>
        /// <param name="format">Export format (json or csv, default: json)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Exported dashboard data file</returns>
        [HttpGet("export")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ExportDashboardData(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Daily,
            [FromQuery] string format = "json",
            CancellationToken cancellationToken = default)
        {
            var dateRange = new AnalyticsDateRange
            {
                StartDate = startDate,
                EndDate = endDate,
                Period = period
            };

            var result = await _analyticsService.ExportDashboardDataAsync(dateRange, format, cancellationToken);

            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            var contentType = format.Equals("csv", StringComparison.OrdinalIgnoreCase)
                ? "text/csv"
                : "application/json";

            var fileName = $"analytics-dashboard-{DateTime.UtcNow:yyyyMMdd-HHmmss}.{format}";

            return File(result.Value, contentType, fileName);
        }
    }
}
