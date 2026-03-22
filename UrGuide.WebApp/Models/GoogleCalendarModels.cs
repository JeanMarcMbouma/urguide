using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UrGuide.WebApp.Models
{
    // ── Google Token Endpoint Response ─────────────────────────────────────────

    internal sealed class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = null!;

        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }

    // ── Google Calendar Events API Response ────────────────────────────────────

    internal sealed class GoogleCalendarEventList
    {
        [JsonPropertyName("items")]
        public List<GoogleCalendarEvent>? Items { get; set; }

        [JsonPropertyName("error")]
        public GoogleApiError? Error { get; set; }
    }

    internal sealed class GoogleCalendarEvent
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("start")]
        public GoogleEventDateTime? Start { get; set; }

        [JsonPropertyName("end")]
        public GoogleEventDateTime? End { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    internal sealed class GoogleEventDateTime
    {
        /// <summary>All-day event date (yyyy-MM-dd).</summary>
        [JsonPropertyName("date")]
        public string? Date { get; set; }

        /// <summary>Timed event (RFC 3339).</summary>
        [JsonPropertyName("dateTime")]
        public string? DateTime { get; set; }

        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }
    }

    internal sealed class GoogleApiError
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    // ── Public API Models ──────────────────────────────────────────────────────

    /// <summary>Returns the current Google Calendar connection state for the guide.</summary>
    public sealed class GoogleCalendarStatusResponse
    {
        public bool IsConnected { get; set; }
        public string? Scope { get; set; }
        public string? ExpiresAt { get; set; }
    }

    /// <summary>Result of syncing Google Calendar events as blocked dates.</summary>
    public sealed class GoogleCalendarSyncResponse
    {
        public int DatesBlocked { get; set; }
        public int DatesSkipped { get; set; }
        public System.Collections.Generic.List<string> BlockedDates { get; set; } = new();
    }
}
