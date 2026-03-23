using System;

namespace UrGuide.Data.Entities.Recommendations
{
    public class UserPreference
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PreferenceType { get; set; } // "category", "location", "price_range", "duration", "language"
        public string PreferenceValue { get; set; }
        public decimal Weight { get; set; } = 1.0m;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
    }
}
