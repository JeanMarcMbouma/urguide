using Plugin.SharedTransitions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UrGuide.Mobile.Contracts
{
    public interface INavigationService
    {
        Task PushAsync(Page page, bool animated);
        Task PushModalAsync(Page modalPage, bool animated);
        Task GotoAsync(string uri);
    }
}
