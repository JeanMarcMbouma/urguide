using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Data.Entities.Shared;

namespace UrGuide.Data.Entities.Posts
{
    public class Category : IAttributeEnabledEntity
    {
        public Category()
        {
            Attributes = new HashSet<GenericAttribute>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public Image Image { get; set; }
        public bool Archived { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
    }
}