using System;
using System.Collections.Generic;

namespace UrGuide.Model.Analytics
{
    public class RevenueMetrics
    {
        public decimal TotalRevenue { get; set; }
        public decimal PlatformFees { get; set; }
        public decimal GuidePayout { get; set; }
        public decimal RefundedAmount { get; set; }
        public decimal NetRevenue { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageTransactionValue { get; set; }
        public List<RevenueDataPoint> TrendData { get; set; } = new List<RevenueDataPoint>();
        public List<RevenueByMethod> PaymentMethodBreakdown { get; set; } = new List<RevenueByMethod>();
    }

    public class RevenueDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal PlatformFees { get; set; }
        public int TransactionCount { get; set; }
    }

    public class RevenueByMethod
    {
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }
}
