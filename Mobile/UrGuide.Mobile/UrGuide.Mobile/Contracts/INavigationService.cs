using Plugin.SharedTransitions;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

namespace UrGuide.Mobile.Contracts
{
    public interface INavigationService
    {
        void GoToRoot();
        Task PushAsync(Page page, bool animated);
        Task PushModalAsync(Page modalPage, bool animated);
    }
}
