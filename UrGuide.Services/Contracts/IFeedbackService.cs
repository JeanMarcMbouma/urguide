using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Contracts
{
    public interface IFeedbackService
    {
        Task<Result<bool>> AddPostFeedbackAsync(string postId, FeedbackModel feedback, CancellationToken cancellationToken);
        Task<Result<bool>> AddUserFeedbackAsync(string userId, FeedbackModel feedback, CancellationToken cancellationToken);
    }
}
