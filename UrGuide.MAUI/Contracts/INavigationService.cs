using System;
using System.Threading.Tasks;
using UrGuide.MAUI.Services;

namespace UrGuide.MAUI.Contracts
{
    public interface INavigationService
    {
        Task ConfirmAsync(Action<DialogResult> callback, string title = null, string message = null, string yesText = "Yes", string noText = "No", bool displayNoButton = true);
        Task DisplayErrorAsync(string title = "Error", string message = "An unexpected error has occured", string yesText = "Ok");
        Task PushAsync(object page, bool animated = true);
        Task PushModalAsync(object modalPage, bool animated = true);
        Task PopModalAsync(bool animated = true);
        Task PushAsyncWithSharedTransition(object page, string groupId);
        Task PopAsync();
    }
}
