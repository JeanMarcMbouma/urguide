using System.Linq;
using UrGuide.Data.Entities.Posts;

namespace UrGuide.Services.Posts
{

    class PostWrapper
    {
        public Post Data { get; }
        public bool HasUserReserved { get; }
        public bool HasReacted { get; }
        public UserReaction.ReactionType ReactionType { get;  }
        public int BidCount { get; set; }
        public int ItineraryCount { get; set; }
        public PostWrapper(Post data, string userId)
        {
            Data = data;
            HasUserReserved = !string.IsNullOrEmpty(userId) && data.Reservations.Any(r => r.UserId == userId);
            HasReacted = !string.IsNullOrEmpty(userId) && data.UserReactions.Any(r => r.UserId == userId);
            BidCount = data.BidCount;
            ItineraryCount = data.Itineraries.Count();
            ReactionType =  data.UserReactions.FirstOrDefault(r => r.UserId == userId)?.Type ?? UserReaction.ReactionType.Neutral;
        }
    }
}
