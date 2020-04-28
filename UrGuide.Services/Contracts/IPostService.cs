using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IPostService
    {
        Task<Result<PostModel>> CreatePostAsync(PostCreationModel model, CancellationToken cancellationToken);
        Task<Result<IEnumerable<PostModel>>> GetLast10PostsAsync(CancellationToken cancellationToken);
        Task<Result<bool>> UpdatePostAsync(PostUpdateModel model, CancellationToken cancellationToken);
        Task<Result<bool>> DeletePostAsync(string id, CancellationToken cancellationToken);
        Task<Result<bool>> UpdatePostAttributesAsync(string id, SetAttribute[] attributes, CancellationToken cancellationToken);
    }
}
