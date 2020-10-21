using System;
using UrGuide.Core.Contracts;

namespace UrGuide.Data.Entities.Posts
{
    public class BidHistory : IEntity
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public virtual Users.User Author { get; set; }
        public string Value { get; set; }
    }
}
