using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AvailabilityController> _logger;
        private const string DateFormat = "yyyy-MM-dd";

        // In-memory storage per guide (in production this would be persisted to DB)
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> _blockedDates = new();
        private static readonly ConcurrentDictionary<string, RecurringPatternRequest> _recurringPatterns = new();

        public AvailabilityController(ILogger<AvailabilityController> logger)
        {
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Get availability slots for a date range
        /// </summary>
        [HttpGet]
        public IActionResult GetAvailability([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format. Use yyyy-MM-dd." });

                var slots = new List<AvailabilitySlot>();
                var blocked = _blockedDates.TryGetValue(guideId, out var set) ? set : new ConcurrentDictionary<string, bool>();
                var pattern = _recurringPatterns.TryGetValue(guideId, out var p) ? p : null;

                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    var dateStr = date.ToString(DateFormat);
                    bool isBlocked = blocked.ContainsKey(dateStr);

                    // Apply recurring pattern
                    if (!isBlocked && pattern != null)
                    {
                        DateTime patternEnd = DateTime.MaxValue;
                        if (pattern.EndDate != null)
                            DateTime.TryParseExact(pattern.EndDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out patternEnd);

                        if (date <= patternEnd)
                        {
                            if (pattern.Type == "weekly" && pattern.DayOfWeek.HasValue && (int)date.DayOfWeek == pattern.DayOfWeek.Value)
                                isBlocked = true;
                            else if (pattern.Type == "monthly" && pattern.DayOfMonth.HasValue && date.Day == pattern.DayOfMonth.Value)
                                isBlocked = true;
                        }
                    }

                    slots.Add(new AvailabilitySlot
                    {
                        Date = dateStr,
                        IsBlocked = isBlocked,
                        RecurringPattern = isBlocked && pattern != null ? pattern.Type : null,
                    });                }

                return Ok(new AvailabilityResponse { Slots = slots, StartDate = startDate, EndDate = endDate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving availability");
                return StatusCode(500, new { error = "An error occurred while retrieving availability" });
            }
        }

        /// <summary>
        /// Block a range of dates
        /// </summary>
        [HttpPost("block")]
        public IActionResult BlockDates([FromBody] BlockDatesRequest request)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(request.StartDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(request.EndDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format." });

                var guideBlocked = _blockedDates.GetOrAdd(guideId, _ => new ConcurrentDictionary<string, bool>());

                for (var date = start; date <= end; date = date.AddDays(1))
                    guideBlocked[date.ToString(DateFormat)] = true;

                return Ok(new { message = "Dates blocked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking dates");
                return StatusCode(500, new { error = "An error occurred while blocking dates" });
            }
        }

        /// <summary>
        /// Unblock a range of dates
        /// </summary>
        [HttpDelete("block")]
        public IActionResult UnblockDates([FromQuery] string startDate, [FromQuery] string endDate)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format." });

                if (_blockedDates.TryGetValue(guideId, out var blockedSet))
                {
                    for (var date = start; date <= end; date = date.AddDays(1))
                        blockedSet.TryRemove(date.ToString(DateFormat), out _);
                }

                return Ok(new { message = "Dates unblocked successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking dates");
                return StatusCode(500, new { error = "An error occurred while unblocking dates" });
            }
        }

        /// <summary>
        /// Set recurring availability pattern
        /// </summary>
        [HttpPost("recurring")]
        public IActionResult SetRecurringPattern([FromBody] RecurringPatternRequest request)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _recurringPatterns[guideId] = request;
                return Ok(new { message = "Recurring pattern set successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting recurring pattern");
                return StatusCode(500, new { error = "An error occurred while setting the recurring pattern" });
            }
        }

        /// <summary>
        /// Clear recurring availability pattern
        /// </summary>
        [HttpDelete("recurring")]
        public IActionResult ClearRecurringPattern()
        {
            var guideId = GetUserId();
            if (string.IsNullOrEmpty(guideId)) return Unauthorized();
            _recurringPatterns.TryRemove(guideId, out _);
            return Ok(new { message = "Recurring pattern cleared." });
        }
    }
}
