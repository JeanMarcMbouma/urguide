using Plugin.SharedTransitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

namespace UrGuide.Mobile.Services
{
    class NavigationService : INavigationService
    {
        public Task GotoAsync(string uri)
        {
            return Shell.Current.GoToAsync(uri);
        }

        public Task PushAsync(Page page, bool animated)
        {
            return Shell.Current.Navigation.PushAsync(page, animated);
        }

        public Task PushModalAsync(Page modalPage, bool animated)
        {
            return Shell.Current.Navigation.PushModalAsync(modalPage, animated);
        }
    }
}
