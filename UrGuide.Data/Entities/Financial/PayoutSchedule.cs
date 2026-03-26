using System;

namespace UrGuide.Data.Entities.Financial
{
    public enum PayoutFrequency
    {
        Weekly = 0,
        BiWeekly = 1,
        Monthly = 2,
        OnDemand = 3
    }

    public enum PayoutScheduleStatus
    {
        Active = 0,
        Paused = 1,
        Cancelled = 2
    }

    public class PayoutSchedule
    {
        public string PayoutScheduleId { get; set; }
        public string GuideId { get; set; }
        public PayoutFrequency Frequency { get; set; }
        public decimal MinimumAmount { get; set; }
        public DateTime NextPayoutDate { get; set; }
        public DateTime? LastPayoutDate { get; set; }
        public PayoutScheduleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
