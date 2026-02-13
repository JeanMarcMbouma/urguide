using System;

namespace UrGuide.Model.Admin
{
    /// <summary>
    /// User activity information for admin dashboard
    /// </summary>
    public class UserActivityModel
    {
        public string UserId { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string IpAddress { get; set; }
    }
}
