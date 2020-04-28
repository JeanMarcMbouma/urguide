using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;
using UrGuide.Data.Entities.Contracts;

namespace UrGuide.Data.Entities.Users
{
    public class User : IAttributeEnabledEntity
    {
        public User()
        {
            Attributes = new HashSet<GenericAttribute>();
        }
        public string Id { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public DateTime LastActivityDate { get; set; }
        public virtual Image ProfileImage { get; set; }

    }
}
