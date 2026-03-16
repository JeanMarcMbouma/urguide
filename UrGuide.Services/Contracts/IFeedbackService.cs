using System.Threading;
using System.Threading.Tasks;
using BbQ.Outcome;
using UrGuide.Core;
using UrGuide.Model;
using UrGuide.Model.Results;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Contracts
{
    public interface IFeedbackService
    {
        Task<Outcome<bool>> AddPostFeedbackAsync(string postId, FeedbackModel feedback, CancellationToken cancellationToken);
        Task<Outcome<bool>> AddUserFeedbackAsync(string userId, FeedbackModel feedback, CancellationToken cancellationToken);
        Task<Outcome<PagedList<AuthoredFeedback>>> GetPostFeedback(string postId, PaginationParameters paginationParameters , CancellationToken cancellationToken);
        Task<Outcome<PagedList<AuthoredFeedback>>> GetUserFeedback(string userId, PaginationParameters paginationParameters, CancellationToken cancellationToken);
        Task<Outcome<bool>> RespondToFeedbackAsync(string feedbackId, string guideResponse, CancellationToken cancellationToken);
    }
}
