using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Users
{
    public class GuideVerificationSubmission
    {
        public GuideVerificationSubmission()
        {
            Documents = new HashSet<GuideVerificationDocument>();
        }

        public string Id { get; set; }
        public string GuideId { get; set; }
        public virtual User Guide { get; set; }
        public int Status { get; set; } // maps to GuideVerificationStatus enum
        public DateTime SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string ReviewedByAdminId { get; set; }
        public string AdminNotes { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual ICollection<GuideVerificationDocument> Documents { get; protected set; }
    }
}
