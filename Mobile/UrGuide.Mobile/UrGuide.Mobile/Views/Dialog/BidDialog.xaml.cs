using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views.Dialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BidDialog : ContentPage
    {
        public BidDialog(BidDialogViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}