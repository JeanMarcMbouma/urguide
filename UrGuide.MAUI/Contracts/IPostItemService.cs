using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.MAUI.Models;
using UrGuide.Model.Results;

namespace UrGuide.MAUI.Contracts
{
    public interface IPostItemService
    {
        IObservable<PostItem> Create(UrGuide.MAUI.Models.API.PostCreationModel post);
        Task<Result<IEnumerable<Model.Lookup.CategoryModel>>> GetCategoriesAsync();
        Task<Result<IEnumerable<PostItem>>> GetItemsAsync();
        Task<IEnumerable<PostItem>> GetFavoriteAsync();
        Task<Result<IEnumerable<DiscoverItem>>> SearchAsync(bool nearby, IEnumerable<string> categories = null, string searchTerm = null, int pageNumber = 1);
        Task<Result<PostItem>> GetByIdAsync(string id);
        Task<PageResult<Model.Shared.AuthoredFeedback>> GetPostFeedbackAsync(string id, int pageNumber = 1);
        Task<Result<IEnumerable<Model.Posts.BidHistoryModel>>> GetBidHistoryAsync(string id);
        Task ToggleFavorites(PostItem it);
        Task SetUserReaction(PostItem it);
        Task ToggleReservation(PostItem it);
        Task<Result<Model.Shared.AuthoredFeedback>> SendFeedback(string postId, Model.Shared.FeedbackModel newFeedBack);
        Task<PageResult<PostItem>> GetUserPosts(string userId, int pageNumber);
        Task ShareItem(PostItem it);
        Task<Result<bool>> Bid(string id, double bid);
        Task<Result<string>> AcceptBid(string id);
        Task<Result<string>> RejectBid(string id);
    }
}
