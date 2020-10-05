using Microsoft.AppCenter.Distribute;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Services.Identity;
using Xamarin.Forms;

namespace UrGuide.Mobile
{
    public partial class App : Application
    {
        public App(IMainPageService mainPageService, IIdentityService identity) : base()
        {
            InitializeComponent();
            _= identity.SignInAsync().ConfigureAwait(false);
            MainPage = mainPageService.GetMainPage();
        }

        protected override void OnStart()
        {
            Distribute.CheckForUpdate();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
