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
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileInfo profileInfo)
        {
            InitializeComponent();

            var vm = Forms.Ioc.GetService<ProfileViewModel>();
            BindingContext = vm;
            vm.Load(profileInfo);

        }
    }
}