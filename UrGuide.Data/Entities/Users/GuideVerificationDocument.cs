using System;

namespace UrGuide.Data.Entities.Users
{
    public class GuideVerificationDocument
    {
        public string Id { get; set; }
        public string SubmissionId { get; set; }
        public virtual GuideVerificationSubmission Submission { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public string FileBase64 { get; set; }
        public int Status { get; set; } // 0=submitted, 1=verified, 2=rejected
        public DateTime UploadedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
