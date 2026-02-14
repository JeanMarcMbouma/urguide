using System;

namespace UrGuide.Model.Admin
{
    /// <summary>
    /// Guide information for verification queue
    /// </summary>
    public class PendingGuideVerification
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime RegisteredAt { get; set; }
        public GuideVerificationStatus Status { get; set; }
        public string[] Documents { get; set; }
        public int TourCount { get; set; }
    }

    /// <summary>
    /// Guide verification status enum
    /// </summary>
    public enum GuideVerificationStatus
    {
        Pending = 0,
        UnderReview = 1,
        Approved = 2,
        Rejected = 3
    }

    /// <summary>
    /// Request to approve or reject guide verification
    /// </summary>
    public class GuideVerificationDecisionModel
    {
        public string UserId { get; set; }
        public bool Approve { get; set; }
        public string Reason { get; set; }
        public string AdminNotes { get; set; }
    }

    /// <summary>
    /// Guide verification details including documents
    /// </summary>
    public class GuideVerificationDetail
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime RegisteredAt { get; set; }
        public GuideVerificationStatus Status { get; set; }
        public VerificationChecklist Checklist { get; set; }
        public GuideDocument[] Documents { get; set; }
        public int TourCount { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    /// <summary>
    /// Verification checklist
    /// </summary>
    public class VerificationChecklist
    {
        public bool ProfileComplete { get; set; }
        public bool IdentityDocumentProvided { get; set; }
        public bool ContactVerified { get; set; }
        public bool BackgroundCheckPassed { get; set; }
        public bool ProfileDescriptionAdequate { get; set; }
    }

    /// <summary>
    /// Guide document information
    /// </summary>
    public class GuideDocument
    {
        public string DocumentId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentUrl { get; set; }
        public DateTime UploadedAt { get; set; }
        public bool Verified { get; set; }
    }
}
