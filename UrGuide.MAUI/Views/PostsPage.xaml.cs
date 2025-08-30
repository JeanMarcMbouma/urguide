using UrGuide.MAUI.ViewModels;

namespace UrGuide.MAUI.Views;

public partial class PostsPage : ContentPage
{
    public PostsPage(PostsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}