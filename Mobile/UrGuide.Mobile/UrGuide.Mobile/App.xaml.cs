using UrGuide.Mobile.Contracts;
using Xamarin.Forms;

namespace UrGuide.Mobile
{
    public partial class App : Application
    {

        public App(IMainPageService mainPageService)
        {
            InitializeComponent();

            MainPage = mainPageService.GetMainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
