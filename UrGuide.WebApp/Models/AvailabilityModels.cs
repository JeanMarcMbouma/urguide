using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class AvailabilitySlot
    {
        public string Date { get; set; }
        public bool IsBlocked { get; set; }
        public string BlockReason { get; set; }
        public string RecurringPattern { get; set; }
    }

    public class BlockDatesRequest
    {
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
        public string Reason { get; set; }
    }

    public class UnblockDatesRequest
    {
        [Required]
        public string StartDate { get; set; }
        [Required]
        public string EndDate { get; set; }
    }

    public class RecurringPatternRequest
    {
        [Required]
        [RegularExpression("weekly|monthly")]
        public string Type { get; set; }

        public int? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public string EndDate { get; set; }
    }

    public class AvailabilityResponse
    {
        public List<AvailabilitySlot> Slots { get; set; } = new();
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
