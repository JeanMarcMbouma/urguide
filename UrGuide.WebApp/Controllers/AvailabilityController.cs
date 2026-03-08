using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private const string DateFormat = "yyyy-MM-dd";

        public AvailabilityController(UrGuideContext context, ILogger<AvailabilityController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        [HttpGet]
        public async Task<IActionResult> GetAvailability([FromQuery] string startDate, [FromQuery] string endDate, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                    !DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var end))
                    return BadRequest(new { error = "Invalid date format. Use yyyy-MM-dd." });

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

                return Ok(new AvailabilityResponse { Slots = slots, StartDate = startDate, EndDate = endDate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving availability");
                return StatusCode(500, new { error = "An error occurred while retrieving availability" });
            }
        }

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
    }
}
