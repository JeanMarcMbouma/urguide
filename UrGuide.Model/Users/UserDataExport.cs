using System;
using System.Collections.Generic;
using UrGuide.Model.Shared;

namespace UrGuide.Model.Users
{
    /// <summary>
    /// Comprehensive user data export model that includes all user-related information
    /// </summary>
    public class UserDataExport
    {
        /// <summary>
        /// Export metadata
        /// </summary>
        public DateTime ExportDate { get; set; }
        public string ExportVersion { get; set; } = "1.0";
        
        /// <summary>
        /// User profile information
        /// </summary>
        public UserInfo Profile { get; set; }
        
        /// <summary>
        /// User attributes and settings
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; } = new();
        
        /// <summary>
        /// User's feedback/reviews given to others
        /// </summary>
        public List<AuthoredFeedback> GivenFeedback { get; set; } = new();
        
        /// <summary>
        /// Feedback/reviews received from others
        /// </summary>
        public List<AuthoredFeedback> ReceivedFeedback { get; set; } = new();
        
        /// <summary>
        /// User's galleries/catalogs
        /// </summary>
        public List<object> Galleries { get; set; } = new();
        
        /// <summary>
        /// User's posts
        /// </summary>
        public List<object> Posts { get; set; } = new();
        
        /// <summary>
        /// User's notifications
        /// </summary>
        public List<object> Notifications { get; set; } = new();
        
        /// <summary>
        /// User's activity history
        /// </summary>
        public List<object> ActivityHistory { get; set; } = new();
        
        /// <summary>
        /// User's bids on tours
        /// </summary>
        public List<object> Bids { get; set; } = new();
        
        /// <summary>
        /// Tour requests created by the user
        /// </summary>
        public List<object> TourRequests { get; set; } = new();
        
        /// <summary>
        /// Account metadata
        /// </summary>
        public AccountMetadata Account { get; set; }
    }
    
    /// <summary>
    /// Account metadata for export
    /// </summary>
    public class AccountMetadata
    {
        public string UserId { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsGuide { get; set; }
        public bool IsPremium { get; set; }
        public string Email { get; set; }
    }
}