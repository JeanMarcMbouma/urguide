using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UrGuide.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(Id), nameof(Id))]
    public partial class PostDetailPage : ContentPage
    {
        public string Id { 
            set {
                ((PostDetailViewModel)BindingContext).Id = value;
            } 
        }
        public PostDetailPage()
        {
            InitializeComponent();
            BindingContext = Forms.Ioc.GetService<PostDetailViewModel>();
        }
        public PostDetailPage(PostDetailViewModel vm) : this()
        {
            BindingContext = vm;
        }
    }
}