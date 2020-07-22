using Microsoft.AppCenter.Distribute;
using UrGuide.Mobile.Contracts;
using Xamarin.Forms;

namespace UrGuide.Mobile
{
    public partial class App : Application
    {
        public App(IMainPageService mainPageService) : base()
        {
            InitializeComponent();

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
