using System;
using System.Collections.Generic;

namespace UrGuide.Model.Disputes
{
    public class DisputeDto
    {
        public string DisputeId { get; set; }
        public string BookingId { get; set; }
        public string FiledBy { get; set; }
        public string AgainstUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Resolution { get; set; }
        public decimal? RefundAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<DisputeEvidenceDto> Evidence { get; set; } = new List<DisputeEvidenceDto>();
        public List<DisputeMessageDto> Messages { get; set; } = new List<DisputeMessageDto>();
    }

    public class DisputeEvidenceDto
    {
        public string EvidenceId { get; set; }
        public string SubmittedBy { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class DisputeMessageDto
    {
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsAdminMessage { get; set; }
        public DateTime SentAt { get; set; }
    }
}
