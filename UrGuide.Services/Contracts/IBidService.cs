using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IBidService
    {
        Task<Result<PostModel>> OpenBidAsync(BidModel model, CancellationToken cancellationToken);
        Task<Result<PostModel>> RejectBidAsync(string postId, CancellationToken cancellationToken);
        Task<Result<PostModel>> AcceptBidAsync(string postId, CancellationToken cancellationToken);
        Task<Result<IEnumerable<BidHistoryModel>>> GetBidHistoryAsync(string postId, CancellationToken cancellationToken);
    }
}
