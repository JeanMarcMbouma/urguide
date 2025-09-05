using UrGuide.MAUI.ViewModels;

namespace UrGuide.MAUI.Views;

public partial class CreateTourRequestPage : ContentPage
{
    public CreateTourRequestPage(CreateTourRequestViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is CreateTourRequestViewModel viewModel)
        {
            viewModel.LoadRegionsCommand.Execute(null);
        }
    }
}