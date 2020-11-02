using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows.Input;
using UrGuide.Mobile.API;
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
        private bool _canCreatePost;
        private bool _shouldLogin;
        private ObservableAsPropertyHelper<Location> _location;
        private readonly CompositeDisposable disposables = new CompositeDisposable();
        public int SelectedViewIndex
        {
            get => selectedViewIndex;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedViewIndex, value);
                if (value == 2)
                    Favorite.Load(null);
            }
        }

        public IPreferenceService Preference { get; }
        public PostsViewModel Posts { get; }
        public DiscoverViewModel Discover { get; }
        public ProfileViewModel Profile { get; }
        public FavoriteViewModel Favorite { get; }
        public Location Location => _location.Value;
        public bool IsLoggedIn
        {
            get => _isLoggedIn; set
            {
                this.RaiseAndSetIfChanged(ref _isLoggedIn, value);
            }
        }

        public bool CanCreatePost
        {
            get => _canCreatePost; set
            {
                this.RaiseAndSetIfChanged(ref _canCreatePost, value);
            }
        }
        public bool ShouldLogin
        {
            get => _shouldLogin; 
            set
            {
                this.RaiseAndSetIfChanged(ref _shouldLogin, value);
            }
        }
        public ICommand CreatePostCommand { get; }
        public ICommand LoginCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand MakeReservationCommand { get; }
        public ICommand SharePostCommand { get; }
        public MainPageViewModel(
            INavigationService navigation,
            IPreferenceService preference,
            IIdentityService identity,
            PostsViewModel posts,
            DiscoverViewModel discover,
            ProfileViewModel profile,
            FavoriteViewModel favorite,
            IPostItemService postItemService,
            IMessagingCenter messaging)
        {
            Preference = preference ?? throw new System.ArgumentNullException(nameof(preference));
            Posts = posts ?? throw new System.ArgumentNullException(nameof(posts));
            Discover = discover ?? throw new System.ArgumentNullException(nameof(discover));
            Profile = profile ?? throw new System.ArgumentNullException(nameof(profile));
            Favorite = favorite ?? throw new System.ArgumentNullException(nameof(favorite));
            IsLoggedIn = false;
            ShouldLogin = !IsLoggedIn;

            this.WhenAnyValue(x => x.Preference.UserId, x => x.Preference.Role, (u, r)=> {
                return (IsLoggedIn: !string.IsNullOrEmpty(u), IsGuide: "guide".Equals(r, StringComparison.OrdinalIgnoreCase));
            })
            .Do((data) =>
            {
                IsLoggedIn = data.IsLoggedIn;
                ShouldLogin = !IsLoggedIn;
                CanCreatePost = data.IsLoggedIn && data.IsGuide;
            })
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe()
            .DisposeWith(disposables);

            SharePostCommand = new Command<PostItem>(async it =>
            {
                await postItemService.ShareItem(it);
            });

            MakeReservationCommand = new Command<PostItem>(async it =>
            {
                await postItemService.ToggleReservation(it).ConfigureAwait(true);
            }, it => IsLoggedIn);

            IObservable<Location> observable = Geolocation.GetLocationAsync()
                        .ToObservable();

            CreatePostCommand = new Command(async () =>
            {
                if(string.IsNullOrEmpty(GlobalSetting.Instance.City))
                {
                    await navigation.DisplayErrorAsync(message: "You need to enable location sharing");
                    await observable.Retry();
                    return;
                }
                await navigation.PushAsync(new CreatePost());
            });

            
            observable
            .Catch(Observable.Empty<Location>())
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(x => x)
            .ToProperty(this, x => x.Location, out _location);

            this.WhenAnyValue(x => x.Location)
                .Do(async location =>
                {
                    if (location == null) return;
                    try
                    {

                        var places = await Geocoding.GetPlacemarksAsync(location);
                        var city = places.FirstOrDefault()?.Locality;
                        var country = places.FirstOrDefault()?.CountryName;
                        GlobalSetting.Instance.City = $"{city}, {country}";
                    }
                    catch (Exception e)
                    {
                    }
                }).Catch(Observable.Empty<Location>()).Subscribe();
            

            LoginCommand = new Command(async () =>
            {
                await identity.SignInAsync();
                Load(null);
            });

            SignOutCommand = new Command(async () =>
            {
                await identity.LogoutAsync();
                MainThread.BeginInvokeOnMainThread(() => SelectedViewIndex = 0);
            });

            identity.GetUserInfo().ToObservable().Subscribe().DisposeWith(disposables);

            messaging.Subscribe<CreatePostViewModel, PostCreationModel>(this, nameof(PostCreationModel), (sender, args) =>
            {
                postItemService.Create(args)
                .Do(x => MainThread.BeginInvokeOnMainThread(() => Posts.Items.Insert(0, x)))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe()
                .DisposeWith(disposables);
            });

            posts.SelectCategory = (args) =>
            {
                Discover.Select(args.Name);
                SelectedViewIndex = 1;
            };
        }

        Task<IEnumerable<Placemark>> Get() => Geocoding.GetPlacemarksAsync(null);
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
