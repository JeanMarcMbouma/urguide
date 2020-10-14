using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.Services.Identity;
using UrGuide.Mobile.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace UrGuide.Mobile.ViewModels
{
    class MainPageViewModel : ReactiveObject, INavigatableViewModel
    {
        private int selectedViewIndex = 0;
        private bool _isLoggedIn;
        private bool shouldLogin;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public int SelectedViewIndex
        {
            get => selectedViewIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedViewIndex, value);
            }
        }

        public IPreferenceService Preference { get; }
        public PostsViewModel Posts { get; }
        public DiscoverViewModel Discover { get; }
        public ProfileViewModel Profile { get; }
        public FavoriteViewModel Favorite { get; }
        public bool IsLoggedIn
        {
            get => _isLoggedIn; set
            {
                this.RaiseAndSetIfChanged(ref _isLoggedIn, value);
            }
        }
        public bool ShouldLogin
        {
            get => shouldLogin; 
            set
            {
                this.RaiseAndSetIfChanged(ref shouldLogin, value);
            }
        }
        public ICommand CreatePostCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand SearchCategoryCommand { get; }
        public MainPageViewModel(
            INavigationService navigation,
            IPreferenceService preference,
            IIdentityService identity,
            PostsViewModel posts,
            DiscoverViewModel discover,
            ProfileViewModel profile,
            FavoriteViewModel favorite)
        {
            Preference = preference ?? throw new System.ArgumentNullException(nameof(preference));
            Posts = posts ?? throw new System.ArgumentNullException(nameof(posts));
            Discover = discover ?? throw new System.ArgumentNullException(nameof(discover));
            Profile = profile ?? throw new System.ArgumentNullException(nameof(profile));
            Favorite = favorite ?? throw new System.ArgumentNullException(nameof(favorite));
            IsLoggedIn = false;
            ShouldLogin = !IsLoggedIn;

            this.WhenAnyValue(x => x.Preference.UserId)
                .Do(u =>
                {
                    IsLoggedIn = !string.IsNullOrEmpty(u);
                    ShouldLogin = !IsLoggedIn;
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe()
                .DisposeWith(disposables);
            SearchCategoryCommand = new Command<SearchOption>(option =>
            {
                Discover.Select(option);
                SelectedViewIndex = 1;
            });

            CreatePostCommand = new Command(() => navigation.PushModalAsync(new CreatePost()));
            LoginCommand = new Command(async () =>
            {
                await identity.SignInAsync();
                Profile.Load(null);
            });
            SignOutCommand = new Command(async () =>
            {
                await identity.LogoutAsync();
                MainThread.BeginInvokeOnMainThread(() => SelectedViewIndex = 0);
            });

            identity.GetUserInfo().ToObservable().Subscribe().DisposeWith(disposables);
        }

        public void Load(object parameter)
        {
            Posts.Load(parameter);
            Discover.Load(parameter);
            if(!string.IsNullOrEmpty(Preference.UserId))
                Profile.Load(parameter);
            Favorite.Load(parameter);
        }
    }
}
