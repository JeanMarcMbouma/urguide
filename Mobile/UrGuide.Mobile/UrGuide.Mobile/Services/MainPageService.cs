using Plugin.SharedTransitions;
using System;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

namespace UrGuide.Mobile.Services
{
    class NavigationPageService : IMainPageService
    {
        readonly Lazy<SharedTransitionNavigationPage> _navigationPage = new Lazy<SharedTransitionNavigationPage>(() => 
        new SharedTransitionNavigationPage (new MainPage()));
        public Page GetMainPage() => _navigationPage.Value;
    }
}
