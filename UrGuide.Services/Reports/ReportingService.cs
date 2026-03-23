using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrGuide.Data;
using UrGuide.Data.Entities.Reports;
using UrGuide.Model.Reports;

namespace UrGuide.Services.Reports
{
    public class ReportingService : IReportingService
    {
        private readonly UrGuideContext _context;

        public ReportingService(UrGuideContext context)
        {
            _context = context;
        }

        public async Task<ReportDto> GenerateReportAsync(string userId, GenerateReportRequest request)
        {
            var report = new ReportDefinition
            {
                ReportId = Guid.NewGuid().ToString(),
                Name = request.Name,
                Type = (ReportType)request.Type,
                Format = (ReportFormat)request.Format,
                RequestedBy = userId,
                ParametersJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    request.StartDate,
                    request.EndDate,
                    request.Filters
                }),
                Status = ReportStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.ReportDefinitions.Add(report);

            // For CSV reports, generate data immediately
            if (report.Format == ReportFormat.CSV)
            {
                report.Status = ReportStatus.Processing;
                try
                {
                    report.Status = ReportStatus.Completed;
                    report.CompletedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    report.Status = ReportStatus.Failed;
                    report.ErrorMessage = ex.Message;
                }
            }

            await _context.SaveChangesAsync();

            return MapToDto(report);
        }

        public async Task<ReportDto> GetReportAsync(string reportId)
        {
            var report = await _context.ReportDefinitions
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null)
                return null;

            return MapToDto(report);
        }

        public async Task<(List<ReportListItem> Items, int TotalCount)> GetUserReportsAsync(string userId, int page = 1, int pageSize = 20)
        {
            var query = _context.ReportDefinitions
                .Where(r => r.RequestedBy == userId)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReportListItem
                {
                    Id = r.ReportId,
                    Name = r.Name,
                    Type = (int)r.Type,
                    Status = (int)r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ReportDataDto> GetReportDataAsync(string reportId)
        {
            var report = await _context.ReportDefinitions
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null)
                return null;

            return await GenerateReportDataInternalAsync(report);
        }

        private async Task<ReportDataDto> GenerateReportDataInternalAsync(ReportDefinition report)
        {
            var data = new ReportDataDto
            {
                ReportId = report.ReportId,
                ReportName = report.Name
            };

            // Generate data based on report type
            switch (report.Type)
            {
                case ReportType.GuideEarnings:
                    data.Headers = new List<string> { "Date", "Tour", "Amount", "Status" };
                    var payments = await _context.Payments
                        .Where(p => p.UserId == report.RequestedBy)
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(100)
                        .ToListAsync();
                    foreach (var p in payments)
                    {
                        data.Rows.Add(new List<string>
                        {
                            p.CreatedAt.ToString("yyyy-MM-dd"),
                            p.BookingId ?? "",
                            p.Amount.ToString("F2"),
                            p.Status.ToString()
                        });
                    }
                    break;

                case ReportType.BookingSummary:
                    data.Headers = new List<string> { "BookingId", "Amount", "Status", "Date" };
                    var bookings = await _context.Payments
                        .OrderByDescending(p => p.CreatedAt)
                        .Take(100)
                        .ToListAsync();
                    foreach (var b in bookings)
                    {
                        data.Rows.Add(new List<string>
                        {
                            b.BookingId ?? "",
                            b.Amount.ToString("F2"),
                            b.Status.ToString(),
                            b.CreatedAt.ToString("yyyy-MM-dd")
                        });
                    }
                    break;

                default:
                    data.Headers = new List<string> { "Info" };
                    data.Rows.Add(new List<string> { "Report data generation not yet implemented for this type." });
                    break;
            }

            return data;
        }

        public async Task<ScheduledReportDto> CreateScheduleAsync(string userId, CreateScheduledReportRequest request)
        {
            var schedule = new ScheduledReport
            {
                ScheduleId = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = request.Name,
                ReportType = (ReportType)request.Type,
                Format = (ReportFormat)request.Format,
                Frequency = (ScheduleFrequency)request.Frequency,
                EmailRecipients = request.EmailRecipients,
                ParametersJson = request.Parameters,
                IsActive = true,
                NextRunAt = CalculateNextRunAt((ScheduleFrequency)request.Frequency),
                CreatedAt = DateTime.UtcNow
            };

            _context.ScheduledReports.Add(schedule);
            await _context.SaveChangesAsync();

            return MapToScheduleDto(schedule);
        }

        public async Task<List<ScheduledReportDto>> GetSchedulesAsync(string userId)
        {
            var schedules = await _context.ScheduledReports
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return schedules.Select(MapToScheduleDto).ToList();
        }

        public async Task<ScheduledReportDto> UpdateScheduleAsync(string userId, string scheduleId, CreateScheduledReportRequest request)
        {
            var schedule = await _context.ScheduledReports
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && s.UserId == userId);

            if (schedule == null)
                return null;

            schedule.Name = request.Name;
            schedule.ReportType = (ReportType)request.Type;
            schedule.Format = (ReportFormat)request.Format;
            schedule.Frequency = (ScheduleFrequency)request.Frequency;
            schedule.EmailRecipients = request.EmailRecipients;
            schedule.ParametersJson = request.Parameters;
            schedule.NextRunAt = CalculateNextRunAt((ScheduleFrequency)request.Frequency);

            await _context.SaveChangesAsync();

            return MapToScheduleDto(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(string userId, string scheduleId)
        {
            var schedule = await _context.ScheduledReports
                .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId && s.UserId == userId);

            if (schedule == null)
                return false;

            _context.ScheduledReports.Remove(schedule);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<GuideEarningsReportData> GenerateGuideEarningsDataAsync(string guideId, DateTime startDate, DateTime endDate)
        {
            var payments = await _context.Payments
                .Where(p => p.UserId == guideId && p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .ToListAsync();

            var totalEarnings = payments.Sum(p => p.GuidePayout);
            var bookingCount = payments.Count;

            var topTours = payments
                .Where(p => p.BookingId != null)
                .GroupBy(p => p.BookingId)
                .Select(g => new TopTourEarning
                {
                    TourId = g.Key,
                    TourName = g.Key,
                    Earnings = g.Sum(p => p.GuidePayout),
                    BookingCount = g.Count()
                })
                .OrderByDescending(t => t.Earnings)
                .Take(10)
                .ToList();

            return new GuideEarningsReportData
            {
                TotalEarnings = totalEarnings,
                BookingCount = bookingCount,
                AveragePerBooking = bookingCount > 0 ? totalEarnings / bookingCount : 0,
                TopTours = topTours
            };
        }

        public async Task<BookingSummaryReportData> GenerateBookingSummaryDataAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _context.Payments
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .ToListAsync();

            return new BookingSummaryReportData
            {
                TotalBookings = payments.Count,
                CompletedBookings = payments.Count(p => p.Status == Data.Entities.Payments.PaymentStatus.Succeeded),
                CancelledBookings = payments.Count(p => p.Status == Data.Entities.Payments.PaymentStatus.Cancelled),
                Revenue = payments.Where(p => p.Status == Data.Entities.Payments.PaymentStatus.Succeeded).Sum(p => p.Amount)
            };
        }

        public Task<byte[]> ExportToCsvAsync(ReportDataDto data)
        {
            var sb = new StringBuilder();

            // Write headers
            sb.AppendLine(string.Join(",", data.Headers.Select(EscapeCsvField)));

            // Write rows
            foreach (var row in data.Rows)
            {
                sb.AppendLine(string.Join(",", row.Select(EscapeCsvField)));
            }

            return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "\"\"";

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }

            return field;
        }

        private static DateTime CalculateNextRunAt(ScheduleFrequency frequency)
        {
            var now = DateTime.UtcNow;
            return frequency switch
            {
                ScheduleFrequency.Daily => now.AddDays(1).Date,
                ScheduleFrequency.Weekly => now.AddDays(7 - (int)now.DayOfWeek).Date,
                ScheduleFrequency.Monthly => new DateTime(now.Year, now.Month, 1).AddMonths(1),
                ScheduleFrequency.Quarterly => new DateTime(now.Year, ((now.Month - 1) / 3 + 1) * 3 + 1 > 12 ? 1 : ((now.Month - 1) / 3 + 1) * 3 + 1, 1).AddYears(((now.Month - 1) / 3 + 1) * 3 + 1 > 12 ? 1 : 0),
                _ => now.AddDays(1).Date
            };
        }

        private static ReportDto MapToDto(ReportDefinition report)
        {
            return new ReportDto
            {
                Id = report.ReportId,
                Name = report.Name,
                RequestedBy = report.RequestedBy,
                Type = (int)report.Type,
                Format = (int)report.Format,
                Status = (int)report.Status,
                FileUrl = report.FileUrl,
                CreatedAt = report.CreatedAt,
                CompletedAt = report.CompletedAt
            };
        }

        private static ScheduledReportDto MapToScheduleDto(ScheduledReport schedule)
        {
            return new ScheduledReportDto
            {
                ScheduleId = schedule.ScheduleId,
                Name = schedule.Name,
                ReportType = (int)schedule.ReportType,
                Format = (int)schedule.Format,
                Frequency = (int)schedule.Frequency,
                EmailRecipients = schedule.EmailRecipients,
                Parameters = schedule.ParametersJson,
                IsActive = schedule.IsActive,
                LastRunAt = schedule.LastRunAt,
                NextRunAt = schedule.NextRunAt,
                CreatedAt = schedule.CreatedAt
            };
        }
    }
}
