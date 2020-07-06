using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrGuide.Mobile.Contracts
{
    public interface INavigationService
    {
        Task PushAsync(Page page, bool animated = true);
        Task PushModalAsync(Page modalPage, bool animated = true);
        Task GotoAsync(string uri);
        Task PopModalAsync(bool animated = true);
    }
}
