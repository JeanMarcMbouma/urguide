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
            (BindingContext as PostsViewModel).LoadItemsCommand.Execute(null);
            base.OnAppearing();
        }
    }
}