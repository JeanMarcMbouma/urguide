using UrGuide.Mobile.Services.Identity;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostPage : ContentPage
    {
        public PostPage()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetService<PostsViewModel>();
        }

        protected override void OnAppearing()
        {
            var vm = (BindingContext as PostsViewModel);
            base.OnAppearing();
            AppShell.RemoveWelcomeScreen();
            _ = vm.Init();
        }

        private async void ImageButton_Clicked(object sender, System.EventArgs e)
        {
            var id = Forms.Ioc.GetService<IIdentityService>();
            await id.SignInAsync();
        }
    }
}