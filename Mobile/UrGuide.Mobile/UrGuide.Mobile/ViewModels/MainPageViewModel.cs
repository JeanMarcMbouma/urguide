using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Views;
using Xamarin.Forms;

namespace UrGuide.Mobile.ViewModels
{
    class MainPageViewModel : ReactiveObject, INavigatableViewModel
    {
        private int selectedViewIndex = 0;

        public int SelectedViewIndex
        {
            get => selectedViewIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedViewIndex, value);
            }
        }

        public PostsViewModel Posts { get; }
        public DiscoverViewModel Discover { get; }
        public ProfileViewModel Profile { get; }
        public FavoriteViewModel Favorite { get; }

        public ICommand SearchCategoryCommand { get; }
        public ICommand CreatePostCommand { get; }
        public MainPageViewModel(INavigationService navigation)
        {
            Posts = Forms.Ioc.GetRequiredService<PostsViewModel>();
            Discover = Forms.Ioc.GetRequiredService<DiscoverViewModel>();
            Profile = Forms.Ioc.GetRequiredService<ProfileViewModel>();
            Favorite = Forms.Ioc.GetRequiredService<FavoriteViewModel>();

            SearchCategoryCommand = new Command<SearchOption>(option =>
            {
                Discover.Select(option);
                SelectedViewIndex = 2;
            });

            CreatePostCommand = new Command(() => navigation.PushModalAsync(new CreatePost()));
        }

        public void Load(object parameter)
        {
            Posts.Load(parameter);
            Discover.Load(parameter);
            Profile.Load(parameter);
            Favorite.Load(parameter);
        }
    }
}
