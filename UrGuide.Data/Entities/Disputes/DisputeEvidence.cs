using System;

namespace UrGuide.Data.Entities.Disputes
{
    public class DisputeEvidence
    {
        public string EvidenceId { get; set; }
        public string DisputeId { get; set; }
        public string SubmittedBy { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public virtual Dispute Dispute { get; set; }
    }
}
