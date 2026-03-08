using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Contracts
{
    public interface IFeedbackService
    {
        Task<Result<bool>> AddPostFeedbackAsync(string postId, FeedbackModel feedback, CancellationToken cancellationToken);
        Task<Result<bool>> AddUserFeedbackAsync(string userId, FeedbackModel feedback, CancellationToken cancellationToken);
        Task<Result<PagedList<AuthoredFeedback>>> GetPostFeedback(string postId, PaginationParameters paginationParameters , CancellationToken cancellationToken);
        Task<Result<PagedList<AuthoredFeedback>>> GetUserFeedback(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<Result<bool>> RespondToFeedbackAsync(string feedbackId, string guideResponse, CancellationToken cancellationToken);
    }
}
