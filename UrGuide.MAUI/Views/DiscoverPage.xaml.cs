using UrGuide.MAUI.ViewModels;

namespace UrGuide.MAUI.Views;

public partial class DiscoverPage : ContentPage
{
    public DiscoverPage(DiscoverViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}