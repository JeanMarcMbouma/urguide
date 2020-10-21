using Microsoft.Extensions.DependencyInjection;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePost : ContentPage
    {
        public CreatePost()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetRequiredService<CreatePostViewModel>();

        }
    }

}