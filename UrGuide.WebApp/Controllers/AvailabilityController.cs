using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Tour;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Resources;

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
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private const string DateFormat = "yyyy-MM-dd";
        private const string Rfc3339DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";
        private const int DefaultTokenExpirationSeconds = 3600;
        private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string GoogleCalendarEventsEndpoint = "https://www.googleapis.com/calendar/v3/calendars/primary/events";
        private const string GoogleRevokeEndpoint = "https://oauth2.googleapis.com/revoke";

        public AvailabilityController(
            UrGuideContext context,
            ILogger<AvailabilityController> logger,
            IConfiguration configuration,
            IDataProtectionProvider dataProtectionProvider,
            IHttpClientFactory httpClientFactory,
            IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _dataProtectionProvider = dataProtectionProvider;
            _httpClientFactory = httpClientFactory;
            _localizer = localizer;
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
                    return BadRequest(new { error = _localizer["Availability_InvalidDateFormat"].Value });

                var resolvedTimezone = ResolveTimezone(timezone);

                // Fetch blocked dates with their reasons so the slot can report why a date is blocked.
                var blockedEntries = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= start && d.Date <= end)
                    .Select(d => new { d.Date, d.Reason })
                    .ToListAsync(cancellationToken);

                var blockedDateMap = blockedEntries.ToDictionary(d => d.Date, d => d.Reason);

                var pattern = await _context.GuideRecurringPatterns
                    .FirstOrDefaultAsync(p => p.GuideId == guideId, cancellationToken);

                var slots = new List<AvailabilitySlot>();
                for (var date = start; date <= end; date = date.AddDays(1))
                {
                    bool isExplicitlyBlocked = blockedDateMap.ContainsKey(date.Date);
                    bool isBlockedByPattern = false;

                    if (!isExplicitlyBlocked && pattern != null)
                    {
                        var patternEnd = pattern.EndDate ?? DateTime.MaxValue;
                        if (date <= patternEnd)
                        {
                            if (pattern.PatternType == "weekly" && pattern.DayOfWeek.HasValue && (int)date.DayOfWeek == pattern.DayOfWeek.Value)
                                isBlockedByPattern = true;
                            else if (pattern.PatternType == "monthly" && pattern.DayOfMonth.HasValue && date.Day == pattern.DayOfMonth.Value)
                                isBlockedByPattern = true;
                        }
                    }

                    bool isBlocked = isExplicitlyBlocked || isBlockedByPattern;

                    slots.Add(new AvailabilitySlot
                    {
                        Date = date.ToString(DateFormat),
                        IsBlocked = isBlocked,
                        BlockReason = isExplicitlyBlocked ? blockedDateMap[date.Date] : null,
                        RecurringPattern = isBlockedByPattern ? pattern!.PatternType : null,
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
                return StatusCode(500, new { error = _localizer["Availability_RetrieveError"].Value });
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
                    return BadRequest(new { error = _localizer["Availability_InvalidDate"].Value });

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

                return Ok(new { message = _localizer["Availability_BlockSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking dates");
                return StatusCode(500, new { error = _localizer["Availability_BlockError"].Value });
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
                    return BadRequest(new { error = _localizer["Availability_InvalidDate"].Value });

                var toRemove = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= start && d.Date <= end)
                    .ToListAsync(cancellationToken);

                if (toRemove.Count > 0)
                {
                    _context.GuideBlockedDates.RemoveRange(toRemove);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new { message = _localizer["Availability_UnblockSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unblocking dates");
                return StatusCode(500, new { error = _localizer["Availability_UnblockError"].Value });
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
                return Ok(new { message = _localizer["Availability_RecurringSetSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting recurring pattern");
                return StatusCode(500, new { error = _localizer["Availability_RecurringSetError"].Value });
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

                return Ok(new { message = _localizer["Availability_RecurringClearSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing recurring pattern");
                return StatusCode(500, new { error = _localizer["Availability_GenericError"].Value });
            }
        }

        // ── Conflict Check ─────────────────────────────────────────────────────

        /// <summary>
        /// Check whether a given date is blocked for a guide.
        /// When <paramref name="guideId"/> is supplied the caller may be any authenticated
        /// user (e.g. a tourist checking before booking). When omitted the authenticated
        /// user is treated as the guide, so guides can also check their own calendar.
        /// </summary>
        [HttpGet("check")]
        public async Task<IActionResult> CheckConflict(
            [FromQuery] string date,
            [FromQuery] string? guideId,
            CancellationToken cancellationToken)
        {
            try
            {
                // If no guideId is provided, the authenticated user is the guide being checked.
                var resolvedGuideId = string.IsNullOrWhiteSpace(guideId) ? GetUserId() : guideId;
                if (string.IsNullOrEmpty(resolvedGuideId)) return Unauthorized();

                // When an explicit guideId is provided, verify the user exists to prevent
                // returning misleading 404-like empty responses for unknown IDs.
                if (!string.IsNullOrWhiteSpace(guideId))
                {
                    var guideExists = await _context.Users
                        .AnyAsync(u => u.Id == resolvedGuideId, cancellationToken);
                    if (!guideExists)
                        return NotFound(new { error = string.Format(_localizer["Availability_GuideNotFound"].Value, resolvedGuideId) });
                }

                if (!DateTime.TryParseExact(date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var targetDate))
                    return BadRequest(new { error = _localizer["Availability_InvalidDateFormat"].Value });

                var blockedEntry = await _context.GuideBlockedDates
                    .FirstOrDefaultAsync(d => d.GuideId == resolvedGuideId && d.Date == targetDate.Date, cancellationToken);

                bool hasConflict = blockedEntry != null;
                string? conflictReason = blockedEntry?.Reason;

                if (!hasConflict)
                {
                    var pattern = await _context.GuideRecurringPatterns
                        .FirstOrDefaultAsync(p => p.GuideId == resolvedGuideId, cancellationToken);

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
                return StatusCode(500, new { error = _localizer["Availability_ConflictCheckError"].Value });
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
                return StatusCode(500, new { error = _localizer["Availability_ExportError"].Value });
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
                    return BadRequest(new { error = _localizer["Availability_NoValidDates"].Value });

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
                return StatusCode(500, new { error = _localizer["Availability_ImportError"].Value });
            }
        }

        // ── Google Calendar ────────────────────────────────────────────────────

        /// <summary>
        /// Returns the Google OAuth 2.0 authorisation URL to start the Google Calendar
        /// connection flow. Requires GoogleCalendar:ClientId and GoogleCalendar:RedirectUri
        /// to be configured.
        /// A CSRF-safe state token is generated using ASP.NET Core Data Protection and
        /// expires after 10 minutes.
        /// </summary>
        [HttpGet("google/auth-url")]
        public IActionResult GetGoogleAuthUrl()
        {
            var clientId = _configuration["GoogleCalendar:ClientId"];
            if (string.IsNullOrEmpty(clientId))
                return StatusCode(503, new { error = _localizer["GoogleCalendar_NotConfigured"].Value });

            var redirectUri = _configuration["GoogleCalendar:RedirectUri"];
            if (string.IsNullOrEmpty(redirectUri))
                return StatusCode(503, new { error = _localizer["GoogleCalendar_RedirectUriNotConfigured"].Value });

            var guideId = GetUserId();
            if (string.IsNullOrEmpty(guideId)) return Unauthorized();

            // Generate a time-limited, data-protected state token encoding the user id.
            // This prevents CSRF attacks and does not expose internal user identifiers.
            var stateProtector = _dataProtectionProvider
                .CreateProtector("UrGuide.GoogleCalendarOAuth.State")
                .ToTimeLimitedDataProtector();

            var statePayload = $"{guideId}:{Guid.NewGuid():N}";
            var protectedState = stateProtector.Protect(statePayload, lifetime: TimeSpan.FromMinutes(10));

            var scope = Uri.EscapeDataString(
                "https://www.googleapis.com/auth/calendar " +
                "https://www.googleapis.com/auth/calendar.events");

            var authUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                          $"?client_id={Uri.EscapeDataString(clientId)}" +
                          $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                          "&response_type=code" +
                          $"&scope={scope}" +
                          "&access_type=offline" +
                          "&prompt=consent" +
                          $"&state={Uri.EscapeDataString(protectedState)}";

            return Ok(new { authUrl });
        }

        /// <summary>
        /// OAuth 2.0 callback handler for Google Calendar.
        /// Validates the CSRF state token, exchanges the authorisation code for access/refresh
        /// tokens, then stores the tokens encrypted at rest using ASP.NET Core Data Protection.
        /// </summary>
        [HttpGet("google/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCalendarCallback(
            [FromQuery] string? code,
            [FromQuery] string? error,
            [FromQuery] string? state,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { error = string.Format(_localizer["GoogleCalendar_AuthDenied"].Value, error) });

            if (string.IsNullOrEmpty(code))
                return BadRequest(new { error = _localizer["GoogleCalendar_MissingCode"].Value });

            if (string.IsNullOrEmpty(state))
                return BadRequest(new { error = _localizer["GoogleCalendar_MissingState"].Value });

            // ── Validate CSRF state ────────────────────────────────────────────
            var stateProtector = _dataProtectionProvider
                .CreateProtector("UrGuide.GoogleCalendarOAuth.State")
                .ToTimeLimitedDataProtector();

            string guideId;
            try
            {
                var payload = stateProtector.Unprotect(state, out _);
                // payload = "guideId:nonce"
                guideId = payload.Split(':')[0];
                if (string.IsNullOrEmpty(guideId))
                    throw new InvalidOperationException("Empty guide id in state.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Invalid or expired Google Calendar OAuth state");
                return BadRequest(new { error = _localizer["GoogleCalendar_InvalidState"].Value });
            }

            // ── Exchange authorisation code for tokens ─────────────────────────
            var clientId = _configuration["GoogleCalendar:ClientId"];
            var clientSecret = _configuration["GoogleCalendar:ClientSecret"];
            var redirectUri = _configuration["GoogleCalendar:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
                return StatusCode(503, new { error = _localizer["GoogleCalendar_NotConfigured"].Value });

            GoogleTokenResponse tokenResponse;
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["redirect_uri"] = redirectUri,
                    ["grant_type"] = "authorization_code",
                });

                var httpResponse = await httpClient.PostAsync(GoogleTokenEndpoint, tokenRequest, cancellationToken);
                var responseContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

                tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? throw new InvalidOperationException("Empty response from token endpoint.");

                if (!string.IsNullOrEmpty(tokenResponse.Error))
                {
                    _logger.LogWarning("Google token exchange error: {Error} – {Desc}", tokenResponse.Error, tokenResponse.ErrorDescription);
                    return BadRequest(new { error = string.Format(_localizer["GoogleCalendar_TokenExchangeFailed"].Value, tokenResponse.Error) });
                }

                if (string.IsNullOrEmpty(tokenResponse.AccessToken))
                    throw new InvalidOperationException("No access token in response.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exchanging Google authorisation code");
                return StatusCode(502, new { error = _localizer["GoogleCalendar_ExchangeFailed"].Value });
            }

            // ── Store tokens encrypted at rest ─────────────────────────────────
            var tokenProtector = _dataProtectionProvider.CreateProtector("UrGuide.GoogleCalendarOAuth.Tokens");
            var encryptedAccess = tokenProtector.Protect(tokenResponse.AccessToken);
            var encryptedRefresh = string.IsNullOrEmpty(tokenResponse.RefreshToken)
                ? null
                : tokenProtector.Protect(tokenResponse.RefreshToken);

            var now = DateTime.UtcNow;
            var existing = await _context.GuideGoogleCalendarTokens
                .FirstOrDefaultAsync(t => t.GuideId == guideId, cancellationToken);

            if (existing != null)
            {
                existing.EncryptedAccessToken = encryptedAccess;
                if (encryptedRefresh != null) existing.EncryptedRefreshToken = encryptedRefresh;
                existing.Scope = tokenResponse.Scope ?? string.Empty;
                existing.TokenType = tokenResponse.TokenType ?? "Bearer";
                existing.ExpiresAt = now.AddSeconds(tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : DefaultTokenExpirationSeconds);
                existing.UpdatedAt = now;
            }
            else
            {
                await _context.GuideGoogleCalendarTokens.AddAsync(new GuideGoogleCalendarToken
                {
                    Id = Guid.NewGuid().ToString(),
                    GuideId = guideId,
                    EncryptedAccessToken = encryptedAccess,
                    EncryptedRefreshToken = encryptedRefresh,
                    Scope = tokenResponse.Scope ?? string.Empty,
                    TokenType = tokenResponse.TokenType ?? "Bearer",
                    ExpiresAt = now.AddSeconds(tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : DefaultTokenExpirationSeconds),
                    CreatedAt = now,
                    UpdatedAt = now,
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Google Calendar connected for guide {GuideId}", guideId);

            return Ok(new { message = _localizer["GoogleCalendar_ConnectSuccess"].Value });
        }

        /// <summary>
        /// Returns whether the authenticated guide has connected Google Calendar.
        /// </summary>
        [HttpGet("google/status")]
        public async Task<IActionResult> GetGoogleCalendarStatus(CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                var token = await _context.GuideGoogleCalendarTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.GuideId == guideId, cancellationToken);

                return Ok(new GoogleCalendarStatusResponse
                {
                    IsConnected = token != null,
                    Scope = token?.Scope,
                    ExpiresAt = token?.ExpiresAt.ToString("o"),
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Google Calendar status");
                return StatusCode(500, new { error = _localizer["Availability_GenericError"].Value });
            }
        }

        /// <summary>
        /// Disconnects Google Calendar by revoking the stored tokens and removing them
        /// from the database.
        /// </summary>
        [HttpDelete("google")]
        public async Task<IActionResult> DisconnectGoogleCalendar(CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                var token = await _context.GuideGoogleCalendarTokens
                    .FirstOrDefaultAsync(t => t.GuideId == guideId, cancellationToken);

                if (token == null)
                    return Ok(new { message = _localizer["GoogleCalendar_NotConnected"].Value });

                // Attempt to revoke access at Google's side (best-effort)
                try
                {
                    var tokenProtector = _dataProtectionProvider.CreateProtector("UrGuide.GoogleCalendarOAuth.Tokens");
                    var accessToken = tokenProtector.Unprotect(token.EncryptedAccessToken);

                    var httpClient = _httpClientFactory.CreateClient();
                    await httpClient.PostAsync(
                        $"{GoogleRevokeEndpoint}?token={Uri.EscapeDataString(accessToken)}",
                        null,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to revoke Google token for guide {GuideId} (proceeding with local removal)", guideId);
                }

                _context.GuideGoogleCalendarTokens.Remove(token);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new { message = _localizer["GoogleCalendar_DisconnectSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting Google Calendar");
                return StatusCode(500, new { error = _localizer["Availability_GenericError"].Value });
            }
        }

        /// <summary>
        /// Fetches the guide's upcoming busy events from Google Calendar and blocks those
        /// dates in UrGuide. Date range defaults to the next 30 days.
        /// Automatically refreshes the access token using the stored refresh token if needed.
        /// </summary>
        [HttpPost("google/sync")]
        public async Task<IActionResult> SyncGoogleCalendar(
            [FromQuery] string? startDate,
            [FromQuery] string? endDate,
            CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                var tokenRecord = await _context.GuideGoogleCalendarTokens
                    .FirstOrDefaultAsync(t => t.GuideId == guideId, cancellationToken);

                if (tokenRecord == null)
                    return BadRequest(new { error = _localizer["GoogleCalendar_RequireAuth"].Value });

                var tokenProtector = _dataProtectionProvider.CreateProtector("UrGuide.GoogleCalendarOAuth.Tokens");
                var accessToken = tokenProtector.Unprotect(tokenRecord.EncryptedAccessToken);

                // Refresh the access token if it has expired or will expire in the next minute
                if (tokenRecord.ExpiresAt <= DateTime.UtcNow.AddMinutes(1))
                {
                    if (string.IsNullOrEmpty(tokenRecord.EncryptedRefreshToken))
                        return BadRequest(new { error = _localizer["GoogleCalendar_TokenExpired"].Value });

                    var refreshToken = tokenProtector.Unprotect(tokenRecord.EncryptedRefreshToken);
                    var newTokens = await RefreshAccessTokenAsync(refreshToken, cancellationToken);

                    if (newTokens == null)
                        return StatusCode(502, new { error = _localizer["GoogleCalendar_TokenRefreshFailed"].Value });

                    accessToken = newTokens.AccessToken;
                    tokenRecord.EncryptedAccessToken = tokenProtector.Protect(newTokens.AccessToken);
                    if (!string.IsNullOrEmpty(newTokens.RefreshToken))
                        tokenRecord.EncryptedRefreshToken = tokenProtector.Protect(newTokens.RefreshToken);
                    tokenRecord.ExpiresAt = DateTime.UtcNow.AddSeconds(newTokens.ExpiresIn > 0 ? newTokens.ExpiresIn : DefaultTokenExpirationSeconds);
                    tokenRecord.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var rangeStart = DateTime.UtcNow.Date;
                var rangeEnd = rangeStart.AddDays(30);

                if (!string.IsNullOrEmpty(startDate) &&
                    DateTime.TryParseExact(startDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ps))
                    rangeStart = ps;

                if (!string.IsNullOrEmpty(endDate) &&
                    DateTime.TryParseExact(endDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var pe))
                    rangeEnd = pe;

                // Fetch events from Google Calendar API
                var timeMin = Uri.EscapeDataString(rangeStart.ToString(Rfc3339DateTimeFormat));
                var timeMax = Uri.EscapeDataString(rangeEnd.AddDays(1).ToString(Rfc3339DateTimeFormat));
                var eventsUrl = $"{GoogleCalendarEventsEndpoint}?timeMin={timeMin}&timeMax={timeMax}&singleEvents=true&orderBy=startTime";

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var eventsResponse = await httpClient.GetAsync(eventsUrl, cancellationToken);
                var eventsContent = await eventsResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!eventsResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google Calendar events API returned {Status}: {Content}", eventsResponse.StatusCode, eventsContent);
                    return StatusCode(502, new { error = _localizer["GoogleCalendar_RetrieveEventsFailed"].Value });
                }

                var eventList = JsonSerializer.Deserialize<GoogleCalendarEventList>(eventsContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (eventList?.Error != null)
                    return StatusCode(502, new { error = string.Format(_localizer["GoogleCalendar_ApiError"].Value, eventList.Error.Message) });

                var datesToBlock = new HashSet<DateTime>();
                foreach (var evt in eventList?.Items ?? new List<GoogleCalendarEvent>())
                {
                    if (evt.Status == "cancelled") continue;

                    var evtStart = ParseGoogleEventDate(evt.Start);
                    var evtEnd = ParseGoogleEventDate(evt.End);
                    if (evtStart == null) continue;
                    var evtEndDate = evtEnd ?? evtStart.Value.AddDays(1);

                    for (var d = evtStart.Value.Date; d < evtEndDate.Date; d = d.AddDays(1))
                    {
                        datesToBlock.Add(d);
                    }
                }

                if (datesToBlock.Count == 0)
                    return Ok(new GoogleCalendarSyncResponse());

                var existingDatesList = await _context.GuideBlockedDates
                    .Where(d => d.GuideId == guideId && d.Date >= rangeStart && d.Date <= rangeEnd)
                    .Select(d => d.Date)
                    .ToListAsync(cancellationToken);

                var existingDates = new HashSet<DateTime>(existingDatesList);
                var now = DateTime.UtcNow;
                var toAdd = new List<GuideBlockedDate>();
                var blockedDateStrings = new List<string>();

                foreach (var date in datesToBlock.OrderBy(d => d))
                {
                    if (!existingDates.Contains(date))
                    {
                        toAdd.Add(new GuideBlockedDate
                        {
                            Id = Guid.NewGuid().ToString(),
                            GuideId = guideId,
                            Date = date,
                            Reason = "Synced from Google Calendar",
                            CreatedAt = now,
                        });
                        blockedDateStrings.Add(date.ToString(DateFormat));
                        existingDates.Add(date);
                    }
                }

                if (toAdd.Count > 0)
                {
                    await _context.GuideBlockedDates.AddRangeAsync(toAdd, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Ok(new GoogleCalendarSyncResponse
                {
                    DatesBlocked = toAdd.Count,
                    DatesSkipped = datesToBlock.Count - toAdd.Count,
                    BlockedDates = blockedDateStrings,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing Google Calendar");
                return StatusCode(500, new { error = _localizer["GoogleCalendar_SyncError"].Value });
            }
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        /// <summary>
        /// Uses the stored refresh token to obtain a new access token from Google.
        /// Returns null if the refresh fails.
        /// </summary>
        private async Task<GoogleTokenResponse?> RefreshAccessTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var clientId = _configuration["GoogleCalendar:ClientId"];
            var clientSecret = _configuration["GoogleCalendar:ClientSecret"];
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                return null;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var request = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["refresh_token"] = refreshToken,
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "refresh_token",
                });

                var response = await httpClient.PostAsync(GoogleTokenEndpoint, request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (tokenResponse == null || !string.IsNullOrEmpty(tokenResponse.Error)
                    || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    _logger.LogWarning("Google token refresh failed: {Error}", tokenResponse?.Error);
                    return null;
                }

                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Google token refresh");
                return null;
            }
        }

        /// <summary>Parses a Google Calendar event date/time to a UTC DateTime.</summary>
        private static DateTime? ParseGoogleEventDate(GoogleEventDateTime? dt)
        {
            if (dt == null) return null;

            // All-day event
            if (!string.IsNullOrEmpty(dt.Date) &&
                DateTime.TryParseExact(dt.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var allDay))
                return allDay;

            // Timed event (RFC 3339)
            if (!string.IsNullOrEmpty(dt.DateTime) &&
                DateTimeOffset.TryParse(dt.DateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out var timed))
                return timed.UtcDateTime;

            return null;
        }

        /// <summary>
        /// Resolves an IANA or Windows timezone ID to a <see cref="TimeZoneInfo"/>.
        /// <para>
        /// On Linux, <see cref="TimeZoneInfo.FindSystemTimeZoneById"/> accepts IANA IDs natively.
        /// On Windows (without ICU), it accepts Windows IDs only. This method adds a two-pass
        /// fallback using the built-in <see cref="TimeZoneInfo.TryConvertIanaIdToWindowsId"/> and
        /// <see cref="TimeZoneInfo.TryConvertWindowsIdToIanaId"/> helpers (available in .NET 6+)
        /// so that both ID styles work on every platform.
        /// </para>
        /// Returns <see cref="TimeZoneInfo.Utc"/> when the ID cannot be resolved.
        /// </summary>
        private static TimeZoneInfo ResolveTimezone(string? ianaOrWindowsId)
        {
            if (string.IsNullOrWhiteSpace(ianaOrWindowsId))
                return TimeZoneInfo.Utc;

            // Direct lookup — works as-is on Linux (IANA) and Windows ≥ .NET 6 with ICU enabled.
            try { return TimeZoneInfo.FindSystemTimeZoneById(ianaOrWindowsId); }
            catch { }

            // Fallback for Windows without ICU: convert IANA → Windows ID.
            if (TimeZoneInfo.TryConvertIanaIdToWindowsId(ianaOrWindowsId, out var windowsId))
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(windowsId); }
                catch { }
            }

            // Fallback for IANA-only environments receiving a Windows ID: convert Windows → IANA.
            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(ianaOrWindowsId, out var ianaId))
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(ianaId); }
                catch { }
            }

            return TimeZoneInfo.Utc;
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

            // RFC 5545 requires CRLF line endings; use Append with "\r\n" for strict conformance.
            sb.Append("BEGIN:VCALENDAR\r\n");
            sb.Append("VERSION:2.0\r\n");
            sb.Append("PRODID:-//UrGuide//Availability Calendar//EN\r\n");
            sb.Append("CALSCALE:GREGORIAN\r\n");
            sb.Append("METHOD:PUBLISH\r\n");

            foreach (var bd in blockedDates)
            {
                var dtStart = bd.Date.ToString("yyyyMMdd");
                var dtEnd = bd.Date.AddDays(1).ToString("yyyyMMdd");
                var uid = $"{bd.Date:yyyyMMdd}-{guideId}@availability.urguide";
                var summary = string.IsNullOrWhiteSpace(bd.Reason) ? "Unavailable" : $"Unavailable: {bd.Reason}";

                sb.Append("BEGIN:VEVENT\r\n");
                sb.Append($"UID:{uid}\r\n");
                sb.Append($"DTSTAMP:{stamp}\r\n");
                sb.Append($"DTSTART;VALUE=DATE:{dtStart}\r\n");
                sb.Append($"DTEND;VALUE=DATE:{dtEnd}\r\n");
                sb.Append($"SUMMARY:{EscapeIcalText(summary)}\r\n");
                sb.Append("TRANSP:OPAQUE\r\n");
                sb.Append("END:VEVENT\r\n");
            }

            // Expand recurring pattern into individual VEVENT entries
            if (pattern != null)
            {
                var patternEnd = pattern.EndDate ?? rangeEnd;
                for (var date = rangeStart; date <= patternEnd && date <= rangeEnd; date = date.AddDays(1))
                {
                    bool isWeeklyMatch = pattern.PatternType == "weekly"
                        && pattern.DayOfWeek.HasValue
                        && (int)date.DayOfWeek == pattern.DayOfWeek.Value;

                    bool isMonthlyMatch = pattern.PatternType == "monthly"
                        && pattern.DayOfMonth.HasValue
                        && date.Day == pattern.DayOfMonth.Value;

                    if (!isWeeklyMatch && !isMonthlyMatch) continue;

                    var dtStart = date.ToString("yyyyMMdd");
                    var dtEnd = date.AddDays(1).ToString("yyyyMMdd");
                    var uid = $"recurring-{date:yyyyMMdd}-{guideId}@availability.urguide";
                    var patternLabel = pattern.PatternType == "weekly"
                        ? $"every {(DayOfWeek)pattern.DayOfWeek!.Value}"
                        : $"day {pattern.DayOfMonth} of every month";

                    sb.Append("BEGIN:VEVENT\r\n");
                    sb.Append($"UID:{uid}\r\n");
                    sb.Append($"DTSTAMP:{stamp}\r\n");
                    sb.Append($"DTSTART;VALUE=DATE:{dtStart}\r\n");
                    sb.Append($"DTEND;VALUE=DATE:{dtEnd}\r\n");
                    sb.Append($"SUMMARY:Unavailable (recurring {patternLabel})\r\n");
                    sb.Append("TRANSP:OPAQUE\r\n");
                    sb.Append("END:VEVENT\r\n");
                }
            }

            sb.Append("END:VCALENDAR\r\n");
            return sb.ToString();
        }

        /// <summary>
        /// Parses DTSTART dates from VEVENT entries in a VCALENDAR string.
        /// Supports VALUE=DATE (yyyyMMdd) and basic DATE-TIME (yyyyMMddTHHmmssZ) formats.
        /// Handles RFC 5545 line folding (continuation lines beginning with space/tab).
        /// </summary>
        private static List<DateTime> ParseIcalDates(string icalContent)
        {
            var dates = new List<DateTime>();
            if (string.IsNullOrWhiteSpace(icalContent)) return dates;

            var normalized = icalContent
                .Replace("\r\n", "\n")
                .Replace("\r", "\n");

            // RFC 5545 §3.1 line unfolding: a CRLF followed by a single space or tab is a
            // line continuation and must be joined to the preceding logical line.
            var physicalLines = normalized.Split('\n');
            var unfoldedLines = new List<string>();
            string? currentLine = null;

            foreach (var physical in physicalLines)
            {
                if (string.IsNullOrEmpty(physical))
                    continue;

                if (currentLine != null && physical.Length > 0 && (physical[0] == ' ' || physical[0] == '\t'))
                {
                    // Remove the leading whitespace (the fold indicator) and join.
                    currentLine += physical[1..];
                }
                else
                {
                    if (currentLine != null)
                        unfoldedLines.Add(currentLine);
                    currentLine = physical;
                }
            }

            if (currentLine != null)
                unfoldedLines.Add(currentLine);

            bool inEvent = false;
            DateTime? dtStart = null;
            DateTime? dtEnd = null;

            foreach (var rawLine in unfoldedLines)
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

        /// <summary>
        /// Extracts the date portion from an iCal DTSTART or DTEND property line.
        /// For DATE-TIME formats the time component is intentionally discarded and the
        /// result is midnight (the date only), since the availability system works at
        /// day granularity. DTEND dates used as range end-points are handled exclusively.
        /// </summary>
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
