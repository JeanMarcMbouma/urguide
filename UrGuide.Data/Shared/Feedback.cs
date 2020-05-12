using System;
using System.Collections;
using System.Collections.Generic;
using UrGuide.Data.Entities.Attributes;

namespace UrGuide.Data.Shared
{
    public class Feedback : Entities.Contracts.ILastUpdatableEntity
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public virtual Entities.Users.User Author { get; set; }
        public int Rating { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; }
    }
}
