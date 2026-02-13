using System;

namespace UrGuide.Model.Admin
{
    /// <summary>
    /// Extended user information for admin dashboard
    /// </summary>
    public class AdminUserInfo
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool IsGuide { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImage { get; set; }
        public string[] Roles { get; set; }
        public int PostCount { get; set; }
        public int TourCount { get; set; }
    }
}
