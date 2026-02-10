using System;

namespace UrGuide.Model.Analytics
{
    public class AnalyticsDateRange
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AnalyticsPeriod Period { get; set; } = AnalyticsPeriod.Daily;
    }

    public enum AnalyticsPeriod
    {
        Hourly = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Yearly = 4
    }
}
