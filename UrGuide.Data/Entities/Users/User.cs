using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using UrGuide.Core.Attributes;
using UrGuide.Core.Contracts;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Data.Shared;

namespace UrGuide.Data.Entities.Users
{
    public class User : IAttributeEnabledEntity, IGeoEntity
    {
        public User()
        {
            Attributes = new HashSet<GenericAttribute>();
            Feedback = new HashSet<Feedback>();
            Notifications = new HashSet<Notification>();
        }
        public string Id { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public virtual ICollection<Feedback> Feedback { get; protected set; }
        public virtual ICollection<Notification> Notifications { get; protected set; }
        public DateTime LastActivityDate { get; set; }
        public virtual Image ProfileImage { get; set; }
        public virtual Point Location { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public object FullName => $"{FirstName} {LastName}";
    }
}
