using System;

namespace UrGuide.Model.Disputes
{
    public class DisputeListItem
    {
        public string DisputeId { get; set; }
        public string BookingId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string FiledBy { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int EvidenceCount { get; set; }
        public int MessageCount { get; set; }
    }
}
