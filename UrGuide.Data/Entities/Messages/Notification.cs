using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Messages
{
    public class Notification
    {
        public Notification()
        {
            Links = new HashSet<Link>();
        }
        public string Id { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public virtual ICollection<Link> Links { get; protected set; }
        public bool Sent { get; set; }
        public bool HasError { get; set; }
        public DateTime Created { get; set; }
    }
}
