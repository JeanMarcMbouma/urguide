using System;
using UrGuide.Data.Entities.Contracts;
using UrGuide.Data.Entities.Users;

namespace UrGuide.Data.Entities.Posts
{
    public class Bid : ILastUpdatableEntity
    {
        public string Id { get; set; }
        public virtual User Author { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
