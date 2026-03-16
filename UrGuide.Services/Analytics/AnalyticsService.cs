using Microsoft.EntityFrameworkCore;
using BbQ.Outcome;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Data.Entities.Payments;
using UrGuide.Data.Entities.Tour;
using UrGuide.Model.Analytics;
using UrGuide.Model.Results;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<AnalyticsService> _logger;

        public AnalyticsService(UrGuideContext context, ILogger<AnalyticsService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Outcome<UserRegistrationTrends>> GetUserRegistrationTrendsAsync(
            AnalyticsDateRange dateRange, 
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddMonths(-6);
                var endDate = dateRange.EndDate ?? DateTime.UtcNow;

                var usersQuery = _context.Users.AsQueryable();

                var totalUsers = await usersQuery.CountAsync(cancellationToken);

                var newUsers = await usersQuery
                    .Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate)
                    .CountAsync(cancellationToken);

                var previousPeriodStart = startDate.AddDays(-(endDate - startDate).TotalDays);
                var previousPeriodUsers = await usersQuery
                    .Where(u => u.CreatedAt >= previousPeriodStart && u.CreatedAt < startDate)
                    .CountAsync(cancellationToken);

                var growthRate = previousPeriodUsers > 0 
                    ? ((decimal)(newUsers - previousPeriodUsers) / previousPeriodUsers) * 100 
                    : 0;

                var trendData = await GetRegistrationTrendData(startDate, endDate, dateRange.Period, cancellationToken);

                return Result.Of(new UserRegistrationTrends
                {
                    TotalUsers = totalUsers,
                    NewUsersInPeriod = newUsers,
                    GrowthRate = growthRate,
                    TrendData = trendData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user registration trends");
                return Result.Of<UserRegistrationTrends>().WithErrors("Failed to retrieve user registration trends");
            }
        }

        public async Task<Outcome<TourBookingStatistics>> GetTourBookingStatisticsAsync(
            AnalyticsDateRange dateRange, 
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddMonths(-6);
                var endDate = dateRange.EndDate ?? DateTime.UtcNow;

                var bookings = await _context.Set<Booking>()
                    .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                    .ToListAsync(cancellationToken);

                var totalBookings = bookings.Count;
                var completedBookings = bookings.Count(b => b.Status == BookingStatus.Confirmed);
                var cancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled);
                var pendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending);
                var avgBookingValue = bookings.Any() ? bookings.Average(b => b.Amount) : 0;

                var trendData = await GetBookingTrendData(startDate, endDate, dateRange.Period, cancellationToken);

                var popularTours = await _context.Set<Booking>()
                    .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                    .Include(b => b.Tour)
                    .GroupBy(b => new { b.TourId, b.Tour.Title })
                    .Select(g => new PopularTour
                    {
                        TourId = g.Key.TourId,
                        TourTitle = g.Key.Title,
                        BookingCount = g.Count(),
                        TotalRevenue = g.Sum(b => b.Amount)
                    })
                    .OrderByDescending(t => t.BookingCount)
                    .Take(10)
                    .ToListAsync(cancellationToken);

                return Result.Of(new TourBookingStatistics
                {
                    TotalBookings = totalBookings,
                    CompletedBookings = completedBookings,
                    CancelledBookings = cancelledBookings,
                    PendingBookings = pendingBookings,
                    AverageBookingValue = avgBookingValue,
                    TrendData = trendData,
                    PopularTours = popularTours
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tour booking statistics");
                return Result.Of<TourBookingStatistics>().WithErrors("Failed to retrieve tour booking statistics");
            }
        }

        public async Task<Outcome<RevenueMetrics>> GetRevenueMetricsAsync(
            AnalyticsDateRange dateRange, 
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddMonths(-6);
                var endDate = dateRange.EndDate ?? DateTime.UtcNow;

                var payments = await _context.Payments
                    .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate && 
                                p.Status == PaymentStatus.Succeeded)
                    .ToListAsync(cancellationToken);

                var totalRevenue = payments.Sum(p => p.Amount);
                var platformFees = payments.Sum(p => p.PlatformFeeAmount);
                var guidePayout = payments.Sum(p => p.GuidePayout);

                var refundedAmount = await _context.Refunds
                    .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
                    .SumAsync(r => r.Amount, cancellationToken);

                var netRevenue = totalRevenue - refundedAmount;
                var transactionCount = payments.Count;
                var avgTransactionValue = transactionCount > 0 ? totalRevenue / transactionCount : 0;

                var trendData = await GetRevenueTrendData(startDate, endDate, dateRange.Period, cancellationToken);

                var paymentMethodBreakdown = payments
                    .GroupBy(p => p.PaymentMethod)
                    .Select(g => new RevenueByMethod
                    {
                        PaymentMethod = g.Key.ToString(),
                        Amount = g.Sum(p => p.Amount),
                        Count = g.Count()
                    })
                    .ToList();

                return Result.Of(new RevenueMetrics
                {
                    TotalRevenue = totalRevenue,
                    PlatformFees = platformFees,
                    GuidePayout = guidePayout,
                    RefundedAmount = refundedAmount,
                    NetRevenue = netRevenue,
                    TransactionCount = transactionCount,
                    AverageTransactionValue = avgTransactionValue,
                    TrendData = trendData,
                    PaymentMethodBreakdown = paymentMethodBreakdown
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue metrics");
                return Result.Of<RevenueMetrics>().WithErrors("Failed to retrieve revenue metrics");
            }
        }

        public async Task<Outcome<GuidePerformanceMetrics>> GetGuidePerformanceMetricsAsync(
            AnalyticsDateRange dateRange, 
            int topN, 
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddMonths(-6);
                var endDate = dateRange.EndDate ?? DateTime.UtcNow;

                var totalGuides = await _context.Set<Data.Entities.Users.Author>().CountAsync(cancellationToken);
                
                var activeGuides = await _context.Set<Data.Entities.Users.Author>()
                    .Where(a => a.Activity != null && a.Activity.LastActive >= startDate)
                    .CountAsync(cancellationToken);

                var avgRating = await _context.Set<Data.Entities.Users.Author>()
                    .Where(a => a.Rating > 0)
                    .AverageAsync(a => (decimal)a.Rating, cancellationToken);

                // Note: This query has N+1 characteristics - consider optimizing for large datasets
                // by using separate grouped queries if performance becomes an issue
                var topPerformers = await _context.Set<Data.Entities.Users.Author>()
                    .Include(a => a.ProfileInfo)
                    .Select(a => new
                    {
                        a.AuthorId,
                        Name = a.ProfileInfo.FirstName ?? "Unknown Guide",
                        Rating = a.Rating,
                        TourCount = _context.Set<Data.Entities.Tour.Tour>().Count(t => t.AuthorId == a.AuthorId),
                        BookingCount = _context.Set<Booking>()
                            .Count(b => b.Tour.AuthorId == a.AuthorId && 
                                       b.CreatedAt >= startDate && 
                                       b.CreatedAt <= endDate),
                        Revenue = _context.Set<Booking>()
                            .Where(b => b.Tour.AuthorId == a.AuthorId && 
                                       b.CreatedAt >= startDate && 
                                       b.CreatedAt <= endDate)
                            .Sum(b => (decimal?)b.Amount) ?? 0,
                        ReviewCount = _context.Set<Data.Entities.Tour.Review>()
                            .Count(r => r.Author.AuthorId == a.AuthorId)
                    })
                    .OrderByDescending(a => a.Revenue)
                    .Take(topN)
                    .ToListAsync(cancellationToken);

                var topGuides = topPerformers.Select(tp => new TopGuide
                {
                    GuideId = tp.AuthorId,
                    GuideName = tp.Name,
                    TotalTours = tp.TourCount,
                    TotalBookings = tp.BookingCount,
                    TotalRevenue = tp.Revenue,
                    AverageRating = tp.Rating,
                    ReviewCount = tp.ReviewCount
                }).ToList();

                return Result.Of(new GuidePerformanceMetrics
                {
                    TotalGuides = totalGuides,
                    ActiveGuides = activeGuides,
                    AverageRating = avgRating,
                    TopPerformers = topGuides
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting guide performance metrics");
                return Result.Of<GuidePerformanceMetrics>().WithErrors("Failed to retrieve guide performance metrics");
            }
        }

        public async Task<Outcome<PopularDestinations>> GetPopularDestinationsAsync(
            AnalyticsDateRange dateRange, 
            int topN, 
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddMonths(-6);
                var endDate = dateRange.EndDate ?? DateTime.UtcNow;

                var destinations = await _context.Set<Booking>()
                    .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                    .Include(b => b.Region)
                    .ThenInclude(r => r.Country)
                    .Include(b => b.Tour)
                    .GroupBy(b => new 
                    { 
                        b.RegionId, 
                        b.Region.Name,
                        CountryName = b.Region.Country.Name 
                    })
                    .Select(g => new DestinationMetric
                    {
                        RegionId = g.Key.RegionId,
                        RegionName = g.Key.Name,
                        CountryName = g.Key.CountryName,
                        TourCount = g.Select(b => b.TourId).Distinct().Count(),
                        BookingCount = g.Count(),
                        Revenue = g.Sum(b => b.Amount),
                        // Calculate average rating separately to avoid N+1 queries
                        AverageRating = 0
                    })
                    .OrderByDescending(d => d.BookingCount)
                    .Take(topN)
                    .ToListAsync(cancellationToken);

                // Calculate average ratings in a separate query for better performance
                // Note: This still creates N queries where N = number of destinations
                // For large datasets, consider batching this into a single query
                foreach (var destination in destinations)
                {
                    var avgRating = await _context.Set<Data.Entities.Tour.Review>()
                        .Where(r => r.Author.ProfileInfo != null && 
                                   _context.Set<Data.Entities.Tour.Tour>()
                                       .Any(t => t.AuthorId == r.Author.AuthorId && t.RegionId == destination.RegionId))
                        .AverageAsync(r => (decimal?)r.Rating, cancellationToken);
                    
                    destination.AverageRating = avgRating ?? 0;
                }

                return Result.Of(new PopularDestinations
                {
                    Destinations = destinations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular destinations");
                return Result.Of<PopularDestinations>().WithErrors("Failed to retrieve popular destinations");
            }
        }

        public async Task<Outcome<ConversionFunnel>> GetConversionFunnelAsync(
            AnalyticsDateRange dateRange, 
            CancellationToken cancellationToken)
        {
            try
            {
                var startDate = dateRange.StartDate ?? DateTime.UtcNow.AddMonths(-6);
                var endDate = dateRange.EndDate ?? DateTime.UtcNow;

                var tourRequests = await _context.TourRequests
                    .Where(tr => tr.CreatedAt >= startDate && tr.CreatedAt <= endDate)
                    .CountAsync(cancellationToken);

                var bidsReceived = await _context.Set<Data.Entities.Posts.Bid>()
                    .Where(b => b.LastUpdated >= startDate && b.LastUpdated <= endDate)
                    .CountAsync(cancellationToken);

                var bookingsCreated = await _context.Set<Booking>()
                    .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                    .CountAsync(cancellationToken);

                var bookingsCompleted = await _context.Set<Booking>()
                    .Where(b => b.CreatedAt >= startDate && b.CreatedAt <= endDate && 
                               b.Status == BookingStatus.Confirmed)
                    .CountAsync(cancellationToken);

                var stages = new List<ConversionStage>
                {
                    new ConversionStage
                    {
                        StageName = "Tour Requests",
                        Count = tourRequests,
                        ConversionRate = 100
                    },
                    new ConversionStage
                    {
                        StageName = "Bids Received",
                        Count = bidsReceived,
                        ConversionRate = tourRequests > 0 ? (decimal)bidsReceived / tourRequests * 100 : 0
                    },
                    new ConversionStage
                    {
                        StageName = "Bookings Created",
                        Count = bookingsCreated,
                        ConversionRate = bidsReceived > 0 ? (decimal)bookingsCreated / bidsReceived * 100 : 0
                    },
                    new ConversionStage
                    {
                        StageName = "Bookings Completed",
                        Count = bookingsCompleted,
                        ConversionRate = bookingsCreated > 0 ? (decimal)bookingsCompleted / bookingsCreated * 100 : 0
                    }
                };

                return Result.Of(new ConversionFunnel
                {
                    TourRequests = tourRequests,
                    BidsReceived = bidsReceived,
                    BidsAccepted = bookingsCreated,
                    BookingsCreated = bookingsCreated,
                    BookingsCompleted = bookingsCompleted,
                    Stages = stages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversion funnel");
                return Result.Of<ConversionFunnel>().WithErrors("Failed to retrieve conversion funnel");
            }
        }

        public async Task<Outcome<DashboardSummary>> GetDashboardSummaryAsync(
            AnalyticsDateRange dateRange, 
            CancellationToken cancellationToken)
        {
            try
            {
                var userTrendsTask = GetUserRegistrationTrendsAsync(dateRange, cancellationToken);
                var bookingStatsTask = GetTourBookingStatisticsAsync(dateRange, cancellationToken);
                var revenueTask = GetRevenueMetricsAsync(dateRange, cancellationToken);
                var guideMetricsTask = GetGuidePerformanceMetricsAsync(dateRange, 10, cancellationToken);
                var destinationsTask = GetPopularDestinationsAsync(dateRange, 10, cancellationToken);
                var funnelTask = GetConversionFunnelAsync(dateRange, cancellationToken);

                await Task.WhenAll(
                    userTrendsTask, 
                    bookingStatsTask, 
                    revenueTask, 
                    guideMetricsTask, 
                    destinationsTask, 
                    funnelTask);

                var userTrends = await userTrendsTask;
                var bookingStats = await bookingStatsTask;
                var revenue = await revenueTask;
                var guideMetrics = await guideMetricsTask;
                var destinations = await destinationsTask;
                var funnel = await funnelTask;

                if (userTrends.IsError || bookingStats.IsError || revenue.IsError || 
                    guideMetrics.IsError || destinations.IsError || funnel.IsError)
                {
                    return Result.Of<DashboardSummary>().WithErrors("Failed to retrieve dashboard summary");
                }

                return Result.Of(new DashboardSummary
                {
                    UserTrends = userTrends.Value,
                    BookingStats = bookingStats.Value,
                    Revenue = revenue.Value,
                    GuideMetrics = guideMetrics.Value,
                    Destinations = destinations.Value,
                    Funnel = funnel.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard summary");
                return Result.Of<DashboardSummary>().WithErrors("Failed to retrieve dashboard summary");
            }
        }

        public async Task<Outcome<byte[]>> ExportDashboardDataAsync(
            AnalyticsDateRange dateRange, 
            string format, 
            CancellationToken cancellationToken)
        {
            try
            {
                var summaryResult = await GetDashboardSummaryAsync(dateRange, cancellationToken);
                
                if (summaryResult.IsError)
                {
                    return Result.Of<byte[]>().WithErrors("Failed to retrieve dashboard data for export");
                }

                byte[] exportData;

                if (format.Equals("json", StringComparison.OrdinalIgnoreCase))
                {
                    var json = JsonSerializer.Serialize(summaryResult.Value, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    exportData = Encoding.UTF8.GetBytes(json);
                }
                else if (format.Equals("csv", StringComparison.OrdinalIgnoreCase))
                {
                    exportData = Encoding.UTF8.GetBytes(GenerateCsvExport(summaryResult.Value));
                }
                else
                {
                    return Result.Of<byte[]>().WithErrors("Unsupported export format. Use 'json' or 'csv'.");
                }

                return Result.Of(exportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting dashboard data");
                return Result.Of<byte[]>().WithErrors("Failed to export dashboard data");
            }
        }

        private async Task<List<RegistrationDataPoint>> GetRegistrationTrendData(
            DateTime startDate, 
            DateTime endDate, 
            AnalyticsPeriod period, 
            CancellationToken cancellationToken)
        {
            var trendData = new List<RegistrationDataPoint>();
            var current = startDate;
            var cumulativeCount = 0;

            while (current <= endDate)
            {
                var nextPeriod = GetNextPeriod(current, period);
                
                var count = await _context.Users
                    .Where(u => u.CreatedAt >= current && u.CreatedAt < nextPeriod)
                    .CountAsync(cancellationToken);

                cumulativeCount += count;

                trendData.Add(new RegistrationDataPoint
                {
                    Date = current,
                    Count = count,
                    CumulativeCount = cumulativeCount
                });

                current = nextPeriod;
            }

            return trendData;
        }

        private async Task<List<BookingDataPoint>> GetBookingTrendData(
            DateTime startDate, 
            DateTime endDate, 
            AnalyticsPeriod period, 
            CancellationToken cancellationToken)
        {
            var trendData = new List<BookingDataPoint>();
            var current = startDate;

            while (current <= endDate)
            {
                var nextPeriod = GetNextPeriod(current, period);
                var bookings = await _context.Set<Booking>()
                    .Where(b => b.CreatedAt >= current && b.CreatedAt < nextPeriod)
                    .ToListAsync(cancellationToken);

                trendData.Add(new BookingDataPoint
                {
                    Date = current,
                    Count = bookings.Count,
                    Revenue = bookings.Sum(b => b.Amount)
                });

                current = nextPeriod;
            }

            return trendData;
        }

        private async Task<List<RevenueDataPoint>> GetRevenueTrendData(
            DateTime startDate, 
            DateTime endDate, 
            AnalyticsPeriod period, 
            CancellationToken cancellationToken)
        {
            var trendData = new List<RevenueDataPoint>();
            var current = startDate;

            while (current <= endDate)
            {
                var nextPeriod = GetNextPeriod(current, period);
                var payments = await _context.Payments
                    .Where(p => p.CreatedAt >= current && p.CreatedAt < nextPeriod && 
                               p.Status == PaymentStatus.Succeeded)
                    .ToListAsync(cancellationToken);

                trendData.Add(new RevenueDataPoint
                {
                    Date = current,
                    Amount = payments.Sum(p => p.Amount),
                    PlatformFees = payments.Sum(p => p.PlatformFeeAmount),
                    TransactionCount = payments.Count
                });

                current = nextPeriod;
            }

            return trendData;
        }

        private DateTime GetNextPeriod(DateTime current, AnalyticsPeriod period)
        {
            return period switch
            {
                AnalyticsPeriod.Hourly => current.AddHours(1),
                AnalyticsPeriod.Daily => current.AddDays(1),
                AnalyticsPeriod.Weekly => current.AddDays(7),
                AnalyticsPeriod.Monthly => current.AddMonths(1),
                AnalyticsPeriod.Yearly => current.AddYears(1),
                _ => current.AddDays(1)
            };
        }

        private string GenerateCsvExport(DashboardSummary summary)
        {
            var csv = new StringBuilder();

            csv.AppendLine("Analytics Dashboard Export");
            csv.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            csv.AppendLine();

            csv.AppendLine("User Registration Summary");
            csv.AppendLine("Total Users,New Users,Growth Rate (%)");
            csv.AppendLine($"{summary.UserTrends.TotalUsers},{summary.UserTrends.NewUsersInPeriod},{summary.UserTrends.GrowthRate:F2}");
            csv.AppendLine();

            csv.AppendLine("Booking Summary");
            csv.AppendLine("Total Bookings,Completed,Cancelled,Pending,Avg Value");
            csv.AppendLine($"{summary.BookingStats.TotalBookings},{summary.BookingStats.CompletedBookings},{summary.BookingStats.CancelledBookings},{summary.BookingStats.PendingBookings},{summary.BookingStats.AverageBookingValue:F2}");
            csv.AppendLine();

            csv.AppendLine("Revenue Summary");
            csv.AppendLine("Total Revenue,Platform Fees,Guide Payout,Refunded,Net Revenue");
            csv.AppendLine($"{summary.Revenue.TotalRevenue:F2},{summary.Revenue.PlatformFees:F2},{summary.Revenue.GuidePayout:F2},{summary.Revenue.RefundedAmount:F2},{summary.Revenue.NetRevenue:F2}");
            csv.AppendLine();

            csv.AppendLine("Guide Performance");
            csv.AppendLine("Total Guides,Active Guides,Average Rating");
            csv.AppendLine($"{summary.GuideMetrics.TotalGuides},{summary.GuideMetrics.ActiveGuides},{summary.GuideMetrics.AverageRating:F2}");
            csv.AppendLine();

            csv.AppendLine("Top Guides");
            csv.AppendLine("Guide Name,Total Tours,Total Bookings,Revenue,Rating");
            foreach (var guide in summary.GuideMetrics.TopPerformers)
            {
                csv.AppendLine($"{guide.GuideName},{guide.TotalTours},{guide.TotalBookings},{guide.TotalRevenue:F2},{guide.AverageRating:F2}");
            }
            csv.AppendLine();

            csv.AppendLine("Popular Destinations");
            csv.AppendLine("Region,Country,Tour Count,Booking Count,Revenue,Rating");
            foreach (var dest in summary.Destinations.Destinations)
            {
                csv.AppendLine($"{dest.RegionName},{dest.CountryName},{dest.TourCount},{dest.BookingCount},{dest.Revenue:F2},{dest.AverageRating:F2}");
            }

            return csv.ToString();
        }
    }
}
