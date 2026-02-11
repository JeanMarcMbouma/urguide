using System;

namespace UrGuide.Data.Entities.Users
{
    public class Notification
    {
        public string Id { get; set; }
        public virtual User Sender { get; set; }
        public string Content { get; set; }
        public string ReferenceLink { get; set; }
        public DateTime Created { get; set; }
        public bool Read { get; set; }
        public bool IsSystem { get; set; }
    }
}
