using UrGuide.Mobile.Contracts;
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
        public Page GetMainPage() => _shell;
    }
}
