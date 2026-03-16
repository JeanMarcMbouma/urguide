using System.Collections.Generic;
using System.Threading;
using BbQ.Outcome;
using System.Threading.Tasks;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;

namespace UrGuide.Services.Contracts
{
    public interface IBidService
    {
        Task<Outcome<PostModel>> OpenBidAsync(BidModel model, CancellationToken cancellationToken);
        Task<Outcome<PostModel>> RejectBidAsync(string postId, CancellationToken cancellationToken);
        Task<Outcome<PostModel>> AcceptBidAsync(string postId, CancellationToken cancellationToken);
        Task<Outcome<IEnumerable<BidHistoryModel>>> GetBidHistoryAsync(string postId, CancellationToken cancellationToken);
    }
}
