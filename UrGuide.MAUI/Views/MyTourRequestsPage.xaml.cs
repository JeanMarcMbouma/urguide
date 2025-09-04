using UrGuide.MAUI.ViewModels;

namespace UrGuide.MAUI.Views;

public partial class MyTourRequestsPage : ContentPage
{
    public MyTourRequestsPage(MyTourRequestsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is MyTourRequestsViewModel viewModel)
        {
            viewModel.LoadTourRequestsCommand.Execute(null);
        }
    }
}