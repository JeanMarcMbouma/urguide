using Plugin.SharedTransitions;
using System;
using System.Threading.Tasks;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Views.Dialog;
using Xamarin.Forms;

namespace UrGuide.Mobile.Services
{
    class NavigationService : INavigationService
    {
        public Task ConfirmAsync(Action<DialogResult> callback, string title = null, string message = null, string yesText = "Yes", string noText = "No", bool displayNoButton = true)
        {
            var modal = new YesNoConfirmation
            {
                Title = title ?? "Warning",
                DisplayText = message ?? "Are you sure?",
                Callback = callback,
                DisplayNoButton = displayNoButton,
                YesText = yesText ?? "Yes",
                NoText = noText ?? "No"
            };
            return PushModalAsync(modal);
        }

        public Task DisplayErrorAsync(string title = "Error", string message = "An error has occured", string yesText = "Ok")
        {
            return ConfirmAsync(null, title, message, yesText, displayNoButton: false);
        }

        public Task PopModalAsync(bool animated = true)
        {
            return Application.Current.MainPage.Navigation.PopModalAsync(animated);
        }

        public Task PushAsync(Page page, bool animated = true)
        {
            return Application.Current.MainPage.Navigation.PushAsync(page, animated);
        }

        public Task PushAsyncWithSharedTransition(Page page, string groupId)
        {
            Page currentPage = (Application.Current.MainPage as SharedTransitionNavigationPage).CurrentPage;
            SharedTransitionNavigationPage.SetTransitionDuration(currentPage, 300);
            SharedTransitionNavigationPage.SetTransitionSelectedGroup(currentPage, groupId);
            return PushAsync(page);
        }

        public Task PushModalAsync(Page modalPage, bool animated = true)
        {
            return Application.Current.MainPage.Navigation.PushModalAsync(modalPage, animated);
        }
    }
}
