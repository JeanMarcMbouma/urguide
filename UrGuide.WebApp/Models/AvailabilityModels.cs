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
        /// <summary>IANA timezone identifier (e.g. "America/New_York"). Defaults to UTC.</summary>
        public string Timezone { get; set; }
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
        [RegularExpression("^(weekly|monthly)$")]
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
        /// <summary>IANA timezone identifier for the response dates.</summary>
        public string Timezone { get; set; } = "UTC";
    }

    /// <summary>Request body for importing availability from an iCal (.ics) string.</summary>
    public class ICalImportRequest
    {
        /// <summary>Raw iCal content (RFC 5545 VCALENDAR format).</summary>
        [Required]
        public string ICalContent { get; set; } = null!;

        /// <summary>Optional reason to attach to all imported blocked dates.</summary>
        public string? Reason { get; set; }
    }

    /// <summary>Result of a booking conflict check for a single date.</summary>
    public class ConflictCheckResponse
    {
        public string Date { get; set; } = string.Empty;
        public bool HasConflict { get; set; }
        public string? ConflictReason { get; set; }
    }

    /// <summary>Response returned after an iCal import.</summary>
    public class ICalImportResponse
    {
        public int DatesImported { get; set; }
        public int DatesSkipped { get; set; }
        public List<string> ImportedDates { get; set; } = new();
    }
}
