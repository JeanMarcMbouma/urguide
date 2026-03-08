using System;

namespace UrGuide.Data.Entities.Tour
{
    public class GuideBlockedDate
    {
        public string Id { get; set; }
        public string GuideId { get; set; }
        public virtual Users.User Guide { get; set; }
        public DateTime Date { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
