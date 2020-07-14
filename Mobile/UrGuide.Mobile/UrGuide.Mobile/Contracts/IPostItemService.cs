using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Mobile.Models;
using UrGuide.Model.Results;

namespace UrGuide.Mobile.Contracts
{
    public interface IPostItemService
    {
        Task<IEnumerable<PostItem>> GetItemsAsync();
        Task<IEnumerable<PostItem>> GetFavoriteAsync();
        Result<IEnumerable<DiscoverItem>> Search(bool nearby, IEnumerable<string> categories = null, string searchTerm = null);
        PostItem GetById(string id);
    }
}
