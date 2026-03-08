using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class VerificationDocumentModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public string Status { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class GuideVerificationStatusResponse
    {
        public string GuideId { get; set; }
        public string OverallStatus { get; set; }
        public List<VerificationDocumentModel> Documents { get; set; } = new();
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string Notes { get; set; }
    }

    public class SubmitVerificationDocumentRequest
    {
        [Required]
        public string DocumentType { get; set; }
        [Required]
        public string FileBase64 { get; set; }
        [Required]
        public string FileName { get; set; }
    }
}
