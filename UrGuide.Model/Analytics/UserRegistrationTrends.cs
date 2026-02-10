using System;
using System.Collections.Generic;

namespace UrGuide.Model.Analytics
{
    public class UserRegistrationTrends
    {
        public int TotalUsers { get; set; }
        public int NewUsersInPeriod { get; set; }
        public decimal GrowthRate { get; set; }
        public List<RegistrationDataPoint> TrendData { get; set; } = new List<RegistrationDataPoint>();
    }

    public class RegistrationDataPoint
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public int CumulativeCount { get; set; }
    }
}
