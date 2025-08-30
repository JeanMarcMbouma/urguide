using UrGuide.MAUI.ViewModels;

namespace UrGuide.MAUI.Views;

public partial class FavoritePage : ContentPage
{
    public FavoritePage(FavoriteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}