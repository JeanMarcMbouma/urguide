using System;

namespace UrGuide.Model.Templates
{
    public class TourTemplateListItem
    {
        public string TemplateId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal BasePrice { get; set; }
        public string CurrencyCode { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
