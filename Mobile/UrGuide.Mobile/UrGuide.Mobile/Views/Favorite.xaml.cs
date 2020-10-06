using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Favorite : ContentPage
    {
        public Favorite()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetService<FavoriteViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as FavoriteViewModel).LoadItemsCommand.Execute(null);
        }
    }
}