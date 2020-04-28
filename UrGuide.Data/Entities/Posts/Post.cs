using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;
using UrGuide.Data.Entities.Shared;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Posts
{
    public class Post
    {
        public Post()
        {
            Attributes = new HashSet<GenericAttribute>();
        }


        public string Id { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
        public DateTime DateOfPublication { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<GenericAttribute> Attributes { get; protected set; }
        public virtual ImageCatalog Catalog { get; set; }
        public virtual User User { get; set; }
    }
}
