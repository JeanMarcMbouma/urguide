using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Discover : ContentPage
    {
        public Discover()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetService<DiscoverViewModel>();
        }
    }
}