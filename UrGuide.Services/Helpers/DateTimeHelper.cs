using System;
using System.Globalization;

namespace UrGuide.Services.Helpers
{
    static class DateTimeHelper
    {
        public static string GetDate(DateTime? value) => value?.ToString("dd-MMM-yyyy", CultureInfo.GetCultureInfo("en-US")) ??  Constants.Unknown;
        public static string GetTime(DateTime? value, DateTimeKind kind = DateTimeKind.Utc) => value.HasValue ? DateTime.SpecifyKind(value.Value, kind).ToString("HH:mm", CultureInfo.GetCultureInfo("en-US")): Constants.Unknown;
        public static string GetDateTime(DateTime? value, DateTimeKind kind = DateTimeKind.Utc) => value.HasValue ? DateTime.SpecifyKind(value.Value, kind).ToString("dd-MMM-yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-US")) : Constants.Unknown;
    }
}
