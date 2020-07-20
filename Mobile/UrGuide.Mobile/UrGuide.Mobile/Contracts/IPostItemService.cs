using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Mobile.Models;
using UrGuide.Model.Results;

namespace UrGuide.Mobile.Contracts
{
    public interface IPostItemService
    {
        Task<Result<IEnumerable<Model.Lookup.CategoryModel>>> GetCategoriesAsync();
        Task<Result<IEnumerable<PostItem>>> GetItemsAsync();
        Task<IEnumerable<PostItem>> GetFavoriteAsync();
        Task<Result<IEnumerable<DiscoverItem>>> SearchAsync(bool nearby, IEnumerable<string> categories = null, string searchTerm = null, int pageNumber = 1);
        Task<Result<PostItem>> GetByIdAsync(string id);
        Task<Result<IEnumerable<Model.Shared.AuthoredFeedback>>> GetPostFeedbackAsync(string id, int pageNumber = 1);
    }
}
