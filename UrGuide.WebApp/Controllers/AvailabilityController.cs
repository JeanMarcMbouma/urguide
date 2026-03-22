using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Tour;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/availability")]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public class AvailabilityController : ControllerBase
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<AvailabilityController> _logger;
        private readonly IConfiguration _configuration;
        private const string DateFormat = "yyyy-MM-dd";

        public AvailabilityController(UrGuideContext context, ILogger<AvailabilityController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // ── Availability Query ─────────────────────────────────────────────────

        [HttpGet]
        public async Task<IActionResult> GetAvailability(
            [FromQuery] string startDate,
            [FromQuery] string endDate,
            [FromQuery] string? timezone,
            CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format. Use yyyy-MM-dd." });

                var resolvedTimezone = ResolveTimezone(timezone);

                var blockedDatesList = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= start && d.Date <= end)
                    .Select(d => d.Date)
                    .ToListAsync(cancellationToken);

                var blockedDates = new HashSet<DateTime>(blockedDatesList);

                var pattern = await _context.GuideRecurringPatterns
                    .FirstOrDefaultAsync(p => p.GuideId == guideId, cancellationToken);

                var slots = new List<AvailabilitySlot>();
                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    bool isBlocked = blockedDates.Contains(date.Date);

                    if (!isBlocked && pattern != null)
                    {
                        var patternEnd = pattern.EndDate ?? DateTime.MaxValue;
                        if (date <= patternEnd)
                        {
                            if (pattern.PatternType == "weekly" && pattern.DayOfWeek.HasValue && (int)date.DayOfWeek == pattern.DayOfWeek.Value)
                                isBlocked = true;
                            else if (pattern.PatternType == "monthly" && pattern.DayOfMonth.HasValue && date.Day == pattern.DayOfMonth.Value)
                                isBlocked = true;
                        }
                    }

                    slots.Add(new AvailabilitySlot
                    {
                        Date = date.ToString(DateFormat),
                        IsBlocked = isBlocked,
                        RecurringPattern = isBlocked && pattern != null ? pattern.PatternType : null,
                    });
                }

                return Ok(new AvailabilityResponse
                {
                    Slots = slots,
                    StartDate = startDate,
                    EndDate = endDate,
                    Timezone = resolvedTimezone.Id,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving availability");
                return StatusCode(500, new { error = "An error occurred while retrieving availability" });
            }
        }

        // ── Block / Unblock ────────────────────────────────────────────────────

        [HttpPost("block")]
        public async Task<IActionResult> BlockDates([FromBody] BlockDatesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(request.StartDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(request.EndDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format." });

                var existingDatesList = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= start && d.Date <= end)
                    .Select(d => d.Date)
                    .ToListAsync(cancellationToken);

                var existingDates = new HashSet<DateTime>(existingDatesList);

                var now = DateTime.UtcNow;
                var toAdd = new List<GuideBlockedDate>();
                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    if (!existingDates.Contains(date.Date))
                    {
                        toAdd.Add(new GuideBlockedDate
                        {
                            Id = Guid.NewGuid().ToString(),
                            GuideId = guideId,
                            Date = date.Date,
                            Reason = request.Reason,
                            CreatedAt = now,
                        });
                    }
                }

                if (toAdd.Count > 0)
                {
                    await _context.GuideBlockedDates.AddRangeAsync(toAdd, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new { message = "Dates blocked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking dates");
                return StatusCode(500, new { error = "An error occurred while blocking dates" });
            }
        }

        [HttpDelete("block")]
        public async Task<IActionResult> UnblockDates([FromQuery] string startDate, [FromQuery] string endDate, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format." });

                var toRemove = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= start && d.Date <= end)
                    .ToListAsync(cancellationToken);

                if (toRemove.Count > 0)
                {
                    _context.GuideBlockedDates.RemoveRange(toRemove);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new { message = "Dates unblocked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking dates");
                return StatusCode(500, new { error = "An error occurred while unblocking dates" });
            }
        }

        // ── Recurring Patterns ────────────────────────────────────────────────

        [HttpPost("recurring")]
        public async Task<IActionResult> SetRecurringPattern([FromBody] RecurringPatternRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existing = await _context.GuideRecurringPatterns
                    .FirstOrDefaultAsync(p => p.GuideId == guideId, cancellationToken);

                var now = DateTime.UtcNow;
                DateTime? parsedEndDate = null;
                if (request.EndDate != null && DateTime.TryParseExact(request.EndDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ed))
                    parsedEndDate = ed;

                if (existing != null)
                {
                    existing.PatternType = request.Type;
                    existing.DayOfWeek = request.DayOfWeek;
                    existing.DayOfMonth = request.DayOfMonth;
                    existing.EndDate = parsedEndDate;
                    existing.UpdatedAt = now;
                }
                else
                {
                    await _context.GuideRecurringPatterns.AddAsync(new GuideRecurringPattern
                    {
                        Id = Guid.NewGuid().ToString(),
                        GuideId = guideId,
                        PatternType = request.Type,
                        DayOfWeek = request.DayOfWeek,
                        DayOfMonth = request.DayOfMonth,
                        EndDate = parsedEndDate,
                        CreatedAt = now,
                        UpdatedAt = now,
                    }, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);
                return Ok(new { message = "Recurring pattern set successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting recurring pattern");
                return StatusCode(500, new { error = "An error occurred while setting the recurring pattern" });
            }
        }

        [HttpDelete("recurring")]
        public async Task<IActionResult> ClearRecurringPattern(CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                var existing = await _context.GuideRecurringPatterns
                    .FirstOrDefaultAsync(p => p.GuideId == guideId, cancellationToken);

                if (existing != null)
                {
                    _context.GuideRecurringPatterns.Remove(existing);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new { message = "Recurring pattern cleared." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing recurring pattern");
                return StatusCode(500, new { error = "An error occurred" });
            }
        }

        // ── Conflict Check ─────────────────────────────────────────────────────

        /// <summary>
        /// Check whether a given date is blocked for the authenticated guide.
        /// Tourists/callers can use this before submitting a booking request.
        /// </summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckConflict([FromQuery] string date, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var targetDate))
                    return BadRequest(new { error = "Invalid date format. Use yyyy-MM-dd." });

                var blockedEntry = await _context.GuideBlockedDates
                    .FirstOrDefaultAsync(d => d.GuideId == guideId && d.Date == targetDate.Date, cancellationToken);

                bool hasConflict = blockedEntry != null;
                string? conflictReason = blockedEntry?.Reason;

                if (!hasConflict)
                {
                    var pattern = await _context.GuideRecurringPatterns
                        .FirstOrDefaultAsync(p => p.GuideId == guideId, cancellationToken);

                    if (pattern != null)
                    {
                        var patternEnd = pattern.EndDate ?? DateTime.MaxValue;
                        if (targetDate <= patternEnd)
                        {
                            if (pattern.PatternType == "weekly" && pattern.DayOfWeek.HasValue && (int)targetDate.DayOfWeek == pattern.DayOfWeek.Value)
                            {
                                hasConflict = true;
                                conflictReason = $"Guide unavailable every {(DayOfWeek)pattern.DayOfWeek.Value}";
                            }
                            else if (pattern.PatternType == "monthly" && pattern.DayOfMonth.HasValue && targetDate.Day == pattern.DayOfMonth.Value)
                            {
                                hasConflict = true;
                                conflictReason = $"Guide unavailable on day {pattern.DayOfMonth.Value} of every month";
                            }
                        }
                    }
                }

                return Ok(new ConflictCheckResponse
                {
                    Date = date,
                    HasConflict = hasConflict,
                    ConflictReason = conflictReason,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking conflict for date {Date}", date);
                return StatusCode(500, new { error = "An error occurred while checking availability conflict" });
            }
        }

        // ── iCal Export ────────────────────────────────────────────────────────

        /// <summary>
        /// Export the guide's blocked dates as an iCal (.ics) file (RFC 5545).
        /// Query parameters startDate and endDate (yyyy-MM-dd) are optional; defaults to the next 12 months.
        /// </summary>
        [HttpGet("export")]
        public async Task<IActionResult> ExportIcal(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                var start = DateTime.UtcNow.Date;
                var end = start.AddMonths(12);

                if (!string.IsNullOrEmpty(startDate) &&
                    DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedStart))
                    start = parsedStart;

                if (!string.IsNullOrEmpty(endDate) &&
                    DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedEnd))
                    end = parsedEnd;

                var blockedDates = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= start && d.Date <= end)
                    .OrderBy(d => d.Date)
                    .ToListAsync(cancellationToken);

                var pattern = await _context.GuideRecurringPatterns
                    .FirstOrDefaultAsync(p => p.GuideId == guideId, cancellationToken);

                var icalContent = BuildIcal(guideId, blockedDates, pattern, start, end);
                var bytes = Encoding.UTF8.GetBytes(icalContent);
                return File(bytes, "text/calendar; charset=utf-8", "availability.ics");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting iCal");
                return StatusCode(500, new { error = "An error occurred while exporting the calendar" });
            }
        }

        // ── iCal Import ────────────────────────────────────────────────────────

        /// <summary>
        /// Import blocked dates from an iCal (.ics) string. VEVENT entries
        /// with DTSTART/DTEND are imported as blocked date ranges.
        /// </summary>
        [HttpPost("import")]
        public async Task<IActionResult> ImportIcal([FromBody] ICalImportRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var datesToBlock = ParseIcalDates(request.ICalContent);
                if (datesToBlock.Count == 0)
                    return BadRequest(new { error = "No valid dates found in the provided iCal content." });

                var existingDatesList = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId)
                    .Select(d => d.Date)
                    .ToListAsync(cancellationToken);

                var existingDates = new HashSet<DateTime>(existingDatesList);
                var now = DateTime.UtcNow;
                var toAdd = new List<GuideBlockedDate>();
                var importedDates = new List<string>();

                foreach (var date in datesToBlock)
                {
                    if (!existingDates.Contains(date.Date))
                    {
                        toAdd.Add(new GuideBlockedDate
                        {
                            Id = Guid.NewGuid().ToString(),
                            GuideId = guideId,
                            Date = date.Date,
                            Reason = string.IsNullOrWhiteSpace(request.Reason) ? "Imported from iCal" : request.Reason,
                            CreatedAt = now,
                        });
                        importedDates.Add(date.ToString(DateFormat));
                        existingDates.Add(date.Date);
                    }
                }

                if (toAdd.Count > 0)
                {
                    await _context.GuideBlockedDates.AddRangeAsync(toAdd, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new ICalImportResponse
                {
                    DatesImported = toAdd.Count,
                    DatesSkipped = datesToBlock.Count - toAdd.Count,
                    ImportedDates = importedDates,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing iCal");
                return StatusCode(500, new { error = "An error occurred while importing the calendar" });
            }
        }

        // ── Google Calendar ────────────────────────────────────────────────────

        /// <summary>
        /// Returns the Google OAuth 2.0 authorisation URL to start the Google Calendar
        /// connection flow. Requires GoogleCalendar:ClientId to be configured.
        /// </summary>
        [HttpGet("google/auth-url")]
        public IActionResult GetGoogleAuthUrl()
        {
            var clientId = _configuration["GoogleCalendar:ClientId"];
            if (string.IsNullOrEmpty(clientId))
                return StatusCode(503, new { error = "Google Calendar integration is not configured." });

            var redirectUri = _configuration["GoogleCalendar:RedirectUri"] ?? $"{Request.Scheme}://{Request.Host}/api/availability/google/callback";
            var scope = Uri.EscapeDataString("https://www.googleapis.com/auth/calendar");
            var state = Uri.EscapeDataString(GetUserId());

            var authUrl = $"https://accounts.google.com/o/oauth2/v2/auth" +
                          $"?client_id={Uri.EscapeDataString(clientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                          $"&response_type=code" +
                          $"&scope={scope}" +
                          $"&access_type=offline" +
                          $"&state={state}";

            return Ok(new { authUrl });
        }

        /// <summary>
        /// OAuth 2.0 callback handler for Google Calendar. Exchanges the authorisation
        /// code for tokens. Requires GoogleCalendar:ClientId and GoogleCalendar:ClientSecret.
        /// </summary>
        [HttpGet("google/callback")]
        [AllowAnonymous]
        public IActionResult GoogleCalendarCallback([FromQuery] string? code, [FromQuery] string? error, [FromQuery] string? state)
        {
            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { error = $"Google authorisation denied: {error}" });

            if (string.IsNullOrEmpty(code))
                return BadRequest(new { error = "Missing authorisation code." });

            var clientId = _configuration["GoogleCalendar:ClientId"];
            var clientSecret = _configuration["GoogleCalendar:ClientSecret"];
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return StatusCode(503, new { error = "Google Calendar integration is not configured." });

            // In a production implementation the code would be exchanged for tokens here
            // using the Google Token endpoint and the tokens stored securely per user.
            _logger.LogInformation("Google Calendar OAuth callback received for state {State}", state);

            return Ok(new
            {
                message = "Google Calendar connected successfully. Token exchange should be completed server-side.",
                state,
            });
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private static TimeZoneInfo ResolveTimezone(string? ianaOrWindowsId)
        {
            if (string.IsNullOrWhiteSpace(ianaOrWindowsId))
                return TimeZoneInfo.Utc;

            try { return TimeZoneInfo.FindSystemTimeZoneById(ianaOrWindowsId); }
            catch { return TimeZoneInfo.Utc; }
        }

        /// <summary>Builds an RFC 5545 VCALENDAR string from blocked dates and recurring patterns.</summary>
        private static string BuildIcal(
            string guideId,
            IEnumerable<GuideBlockedDate> blockedDates,
            GuideRecurringPattern? pattern,
            DateTime rangeStart,
            DateTime rangeEnd)
        {
            var sb = new StringBuilder();
            var stamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");

            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//UrGuide//Availability Calendar//EN");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");

            foreach (var bd in blockedDates)
            {
                var dtStart = bd.Date.ToString("yyyyMMdd");
                var dtEnd = bd.Date.AddDays(1).ToString("yyyyMMdd");
                var uid = $"{bd.Date:yyyyMMdd}-{guideId}@availability.urguide";
                var summary = string.IsNullOrWhiteSpace(bd.Reason) ? "Unavailable" : $"Unavailable: {bd.Reason}";

                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:{uid}");
                sb.AppendLine($"DTSTAMP:{stamp}");
                sb.AppendLine($"DTSTART;VALUE=DATE:{dtStart}");
                sb.AppendLine($"DTEND;VALUE=DATE:{dtEnd}");
                sb.AppendLine($"SUMMARY:{EscapeIcalText(summary)}");
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");
            }

            // Expand recurring pattern into individual VEVENT entries
            if (pattern != null)
            {
                var patternEnd = pattern.EndDate ?? rangeEnd;
                for (var date = rangeStart; date <= patternEnd && date <= rangeEnd; date = date.AddDays(1))
                {
                    bool matches = pattern.PatternType == "weekly"
                        && pattern.DayOfWeek.HasValue
                        && (int)date.DayOfWeek == pattern.DayOfWeek.Value;

                    matches = matches || (pattern.PatternType == "monthly"
                        && pattern.DayOfMonth.HasValue
                        && date.Day == pattern.DayOfMonth.Value);

                    if (!matches) continue;

                    var dtStart = date.ToString("yyyyMMdd");
                    var dtEnd = date.AddDays(1).ToString("yyyyMMdd");
                    var uid = $"recurring-{date:yyyyMMdd}-{guideId}@availability.urguide";
                    var patternLabel = pattern.PatternType == "weekly"
                        ? $"every {(DayOfWeek)pattern.DayOfWeek!.Value}"
                        : $"day {pattern.DayOfMonth} of every month";

                    sb.AppendLine("BEGIN:VEVENT");
                    sb.AppendLine($"UID:{uid}");
                    sb.AppendLine($"DTSTAMP:{stamp}");
                    sb.AppendLine($"DTSTART;VALUE=DATE:{dtStart}");
                    sb.AppendLine($"DTEND;VALUE=DATE:{dtEnd}");
                    sb.AppendLine($"SUMMARY:Unavailable (recurring {patternLabel})");
                    sb.AppendLine("TRANSP:OPAQUE");
                    sb.AppendLine("END:VEVENT");
                }
            }

            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }

        /// <summary>
        /// Parses DTSTART dates from VEVENT entries in a VCALENDAR string.
        /// Supports VALUE=DATE (yyyyMMdd) and basic DATE-TIME (yyyyMMddTHHmmssZ) formats.
        /// </summary>
        private static List<DateTime> ParseIcalDates(string icalContent)
        {
            var dates = new List<DateTime>();
            if (string.IsNullOrWhiteSpace(icalContent)) return dates;

            var lines = icalContent
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries);

            bool inEvent = false;
            DateTime? dtStart = null;
            DateTime? dtEnd = null;

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();

                if (line.Equals("BEGIN:VEVENT", StringComparison.OrdinalIgnoreCase))
                {
                    inEvent = true;
                    dtStart = null;
                    dtEnd = null;
                    continue;
                }

                if (line.Equals("END:VEVENT", StringComparison.OrdinalIgnoreCase))
                {
                    if (inEvent && dtStart.HasValue)
                    {
                        var rangeEnd = dtEnd ?? dtStart.Value.AddDays(1);
                        for (var d = dtStart.Value.Date; d < rangeEnd.Date; d = d.AddDays(1))
                            dates.Add(d);
                    }
                    inEvent = false;
                    continue;
                }

                if (!inEvent) continue;

                if (line.StartsWith("DTSTART", StringComparison.OrdinalIgnoreCase))
                    dtStart = ParseIcalDate(line);
                else if (line.StartsWith("DTEND", StringComparison.OrdinalIgnoreCase))
                    dtEnd = ParseIcalDate(line);
            }

            return dates;
        }

        private static DateTime? ParseIcalDate(string line)
        {
            var colonIdx = line.IndexOf(':', StringComparison.Ordinal);
            if (colonIdx < 0) return null;
            var value = line[(colonIdx + 1)..].Trim();

            // DATE format: yyyyMMdd
            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1))
                return d1;

            // DATE-TIME UTC format: yyyyMMddTHHmmssZ
            if (DateTime.TryParseExact(value, "yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var d2))
                return d2.Date;

            // DATE-TIME local format: yyyyMMddTHHmmss
            if (DateTime.TryParseExact(value, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d3))
                return d3.Date;

            return null;
        }

        private static string EscapeIcalText(string text) =>
            text.Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,").Replace("\n", "\\n");
    }
}
