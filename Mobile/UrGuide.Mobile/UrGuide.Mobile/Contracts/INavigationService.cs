using System;
using System.Threading.Tasks;
using UrGuide.Mobile.Services;
using Xamarin.Forms;

namespace UrGuide.Mobile.Contracts
{
    public interface INavigationService
    {
        Task ConfirmAsync(Action<DialogResult> callback, string title = null, string message = null, string yesText = "Yes", string noText = "No", bool displayNoButton = true);
        Task PushAsync(Page page, bool animated = true);
        Task PushModalAsync(Page modalPage, bool animated = true);
        Task GotoAsync(string uri);
        Task PopModalAsync(bool animated = true);
    }
}
