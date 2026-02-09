using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using UrGuide.Core.Attributes;
using UrGuide.Core.Contracts;
using UrGuide.Data.Entities.Contracts;

namespace UrGuide.Data.Entities.Shared
{
    public class ImageCatalog : IUserOwnedEntity, IAttributeEnabledEntity, IGeoEntity, ILastUpdatableEntity
    {
        public ImageCatalog()
        {
            Images = new HashSet<Image>();
            Attributes = new HashSet<GenericAttribute>();
        }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public virtual Users.User User { get; set; }
        public virtual Posts.Post Post { get; set; }
        public virtual ICollection<Image> Images { get; private set; }
        public virtual ICollection<GenericAttribute> Attributes { get; private set; }
        public virtual Point Location { get; set; }
    }
}