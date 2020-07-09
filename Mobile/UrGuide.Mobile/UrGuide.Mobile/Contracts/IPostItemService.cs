using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.Contracts
{
    public interface IPostItemService
    {
        Task<IEnumerable<PostItem>> GetItemsAsync();
        Task<IEnumerable<PostItem>> GetFavoriteAsync();
        PostItem GetById(string id);
    }
}
