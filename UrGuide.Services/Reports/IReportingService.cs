using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Model.Reports;

namespace UrGuide.Services.Reports
{
    public interface IReportingService
    {
        Task<ReportDto> GenerateReportAsync(string userId, GenerateReportRequest request);
        Task<ReportDto> GetReportAsync(string reportId);
        Task<(List<ReportListItem> Items, int TotalCount)> GetUserReportsAsync(string userId, int page = 1, int pageSize = 20);
        Task<ReportDataDto> GetReportDataAsync(string reportId);
        Task<ScheduledReportDto> CreateScheduleAsync(string userId, CreateScheduledReportRequest request);
        Task<List<ScheduledReportDto>> GetSchedulesAsync(string userId);
        Task<ScheduledReportDto> UpdateScheduleAsync(string userId, string scheduleId, CreateScheduledReportRequest request);
        Task<bool> DeleteScheduleAsync(string userId, string scheduleId);
        Task<GuideEarningsReportData> GenerateGuideEarningsDataAsync(string guideId, DateTime startDate, DateTime endDate);
        Task<BookingSummaryReportData> GenerateBookingSummaryDataAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> ExportToCsvAsync(ReportDataDto data);
    }
}
