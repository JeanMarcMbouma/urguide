using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;
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
        public virtual ICollection<Image> Images { get; protected set; }
        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public virtual Point Location { get; set; }
    }
}