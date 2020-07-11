using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views.Dialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfile : ContentPage
    {
        public EditProfile()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetService<EditProfileViewModel>();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as EditProfileViewModel).LoadCommand.Execute(null);
        }
    }
}