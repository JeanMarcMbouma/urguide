using System;

namespace UrGuide.Data.Entities.Disputes
{
    public class DisputeMessage
    {
        public string MessageId { get; set; }
        public string DisputeId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsAdminMessage { get; set; } = false;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public virtual Dispute Dispute { get; set; }
    }
}
