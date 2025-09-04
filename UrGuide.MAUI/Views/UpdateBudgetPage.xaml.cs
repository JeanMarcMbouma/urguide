using UrGuide.MAUI.Models;
using UrGuide.MAUI.ViewModels;

namespace UrGuide.MAUI.Views;

public partial class UpdateBudgetPage : ContentPage
{
    public UpdateBudgetPage(UpdateBudgetViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}