using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
using UrGuide.Data.Entities.Posts;

namespace UrGuide.Services.Posts
{

    class PostWrapper
    {
        public Post Data { get; }
        public bool HasUserReserved { get; }
        public bool HasReacted { get; }
        public PostWrapper(Post data, string userId)
        {
            Data = data;
            HasUserReserved = !string.IsNullOrEmpty(userId) && data.Reservations.Any(r => r.UserId == userId);
            HasReacted = !string.IsNullOrEmpty(userId) && data.UserReactions.Any(r => r.UserId == userId);
        }
    }
}
