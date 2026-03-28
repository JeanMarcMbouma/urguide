using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Reports;

namespace UrGuide.Services.Reports
{
    public interface IReportingService
    {
        Task<ReportDto> GenerateReportAsync(string userId, GenerateReportRequest request, CancellationToken cancellationToken = default);
        Task<ReportDto> GetReportAsync(string reportId, CancellationToken cancellationToken = default);
        Task<(List<ReportListItem> Items, int TotalCount)> GetUserReportsAsync(string userId, int page = 1, int pageSize = 20, CancellationToken cancellationToken = default);
        Task<ReportDataDto> GetReportDataAsync(string reportId, CancellationToken cancellationToken = default);
        Task<ScheduledReportDto> CreateScheduleAsync(string userId, CreateScheduledReportRequest request, CancellationToken cancellationToken = default);
        Task<List<ScheduledReportDto>> GetSchedulesAsync(string userId, CancellationToken cancellationToken = default);
        Task<ScheduledReportDto> UpdateScheduleAsync(string userId, string scheduleId, CreateScheduledReportRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteScheduleAsync(string userId, string scheduleId, CancellationToken cancellationToken = default);
        Task<GuideEarningsReportData> GenerateGuideEarningsDataAsync(string guideId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<BookingSummaryReportData> GenerateBookingSummaryDataAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<byte[]> ExportToCsvAsync(ReportDataDto data);
    }
}
