using System.Linq;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Discover : ContentView
    {
        //private DiscoverViewModel _vm;

        //public string Category { 
        //    set {
        //        _vm.SearchOptions.First(c => c.Text.Equals(value)).Selected = true;
        //        _vm.SearchCommand.Execute(null);
        //    } 
        //}
        public Discover()
        {
            InitializeComponent();
        }
    }
}