using System.Linq;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(Category), nameof(Category))]

    public partial class Discover : ContentPage
    {
        private DiscoverViewModel _vm;

        public string Category { set {
                _vm.SearchOptions.First(c => c.Text.Equals(value)).Selected = true;
                _vm.SearchCommand.Execute(null);
            } 
        }
        public Discover()
        {
            InitializeComponent();
            _vm = Forms.Ioc.GetService<DiscoverViewModel>();
            BindingContext = _vm;
        }
    }
}