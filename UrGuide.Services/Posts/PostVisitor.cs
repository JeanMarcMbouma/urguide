using System.Collections.Generic;
using System.Linq;
using UrGuide.Data.Entities.Posts;

namespace UrGuide.Services.Posts
{
    class PostVisitor
    {
        public static IEnumerable<PostWrapper> Visit(IEnumerable<Post> posts, string userId)
        {
            return posts.Select(p => new PostWrapper(p, userId));
        }

        public static PostWrapper Visit(Post post, string userId)
        {
            return new PostWrapper(post, userId);
        }
    }
}
