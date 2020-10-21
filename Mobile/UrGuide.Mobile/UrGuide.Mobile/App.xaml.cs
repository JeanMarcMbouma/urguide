using Microsoft.AppCenter.Distribute;
using Microsoft.Extensions.DependencyInjection;
using Plugin.SharedTransitions;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;

namespace UrGuide.Mobile
{
    public partial class App : Application
    {
        public App() : base()
        {
            InitializeComponent();
            MainPage = Forms.Ioc.GetRequiredService<IMainPageService>().GetMainPage();
            if (MainPage is SharedTransitionNavigationPage nav)
            if (nav.CurrentPage.BindingContext is INavigatableViewModel vm)
            {
                vm.Load(null);
            }
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
