using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Data.Entities.Posts
{
    [Owned]
    public class Reservation
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public int Seats { get; set; }
    }

    [Owned]
    public class UserReaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ReactionType Type { get; set; }

        [Flags]
        public enum ReactionType
        {
            Like = 2,
            DisLike = 4
        }
    }
}
