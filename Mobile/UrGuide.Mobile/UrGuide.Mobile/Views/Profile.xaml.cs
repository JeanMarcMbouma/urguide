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
    public partial class Profile : ContentPage
    {
        public Profile()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetService<ProfileViewModel>();
        }

        protected override void OnAppearing()
        {
            (BindingContext as ProfileViewModel).LoadItemsCommand.Execute(null);
            base.OnAppearing();
        }
    }
}