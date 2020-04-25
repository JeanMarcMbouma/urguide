using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;

namespace UrGuide.Data.Entities.Users
{
    public class User
    {
        public string UserId { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; set; }
        public DateTime LastActivityDate { get; set; }
        public virtual Image ProfileImage { get; set; }

    }
}
