using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using UrGuide.Data.Entities.Attributes;
using UrGuide.Data.Entities.Contracts;

namespace UrGuide.Data.Entities.Users
{
    public class User : IAttributeEnabledEntity, IGeoEntity
    {
        public User()
        {
            Attributes = new HashSet<GenericAttribute>();
        }
        public string Id { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public DateTime LastActivityDate { get; set; }
        public virtual Image ProfileImage { get; set; }
        public virtual Point Location { get; set; }
        public object FullName => $"{Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.FirstName))} {Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.LastName))}";
    }
}
