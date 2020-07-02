using Plugin.SharedTransitions;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

namespace UrGuide.Mobile.Services
{
    class MainPageService : IMainPageService
    {
        private readonly AppShell _shell;

        public MainPageService(AppShell shell)
        {
            _shell = shell ?? throw new System.ArgumentNullException(nameof(shell));
        }
        public Page GetMainPage() => Xamarin.Essentials.VersionTracking.IsFirstLaunchEver ? (Page)_shell : new SharedTransitionNavigationPage(new MainPage());
    }
}
