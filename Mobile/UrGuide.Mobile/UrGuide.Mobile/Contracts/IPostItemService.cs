using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Mobile.Models;
using UrGuide.Model.Results;

namespace UrGuide.Mobile.Contracts
{
    public interface IPostItemService
    {
        Task Create(API.PostCreationModel post);
        Task<Result<IEnumerable<Model.Lookup.CategoryModel>>> GetCategoriesAsync();
        Task<Result<IEnumerable<PostItem>>> GetItemsAsync();
        Task<IEnumerable<PostItem>> GetFavoriteAsync();
        Task<Result<IEnumerable<DiscoverItem>>> SearchAsync(bool nearby, IEnumerable<string> categories = null, string searchTerm = null, int pageNumber = 1);
        Task<Result<PostItem>> GetByIdAsync(string id);
        Task<Result<IEnumerable<Model.Shared.AuthoredFeedback>>> GetPostFeedbackAsync(string id, int pageNumber = 1);
        Task<Result<IEnumerable<Model.Posts.BidHistoryModel>>> GetBidHistoryAsync(string id);
        Task ToggleFavorites(PostItem it);
        Task SetUserReaction(PostItem it);
        Task<Result<Model.Shared.AuthoredFeedback>> SendFeedback(string postId, Model.Shared.FeedbackModel newFeedBack);
    }
}
