using MvvmHelpers;
using MvvmHelpers.Commands;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Lookup;

namespace UrGuide.Mobile.ViewModels
{
    public class PostsViewModel : BaseViewModel
    {
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;
        private bool _initialized;
        private PostItem selected;
        private readonly INavigationService _navigation;
        private readonly PostDetailViewModel _detailViewModel;
        private ICommand _viewDetailCommand;
        private ICommand _likeCommand;
        private ICommand _markAsFavoriteCommand;
        private ICommand _loadItemsCommand;
        private ICommand _searchCategoryCommand;

        public ICommand SearchCategoryCommand => _searchCategoryCommand ??= new AsyncCommand<CategoryModel>(
            async (model) => await _navigation.GotoAsync($"//discover?Category={model.Name}"));
        public ICommand ToggleFavoriteCommand => _markAsFavoriteCommand ??= new AsyncCommand<PostItem>(async item =>
        {
            var it = Items.First(x => x.Id == item.Id);
            it.Favorite = !it.Favorite;
            await PostItemService.ToggleFavorites(it);
        });

        public ICommand LoadItemsCommand => _loadItemsCommand ??= new Command(() => IsBusy = true);

        public ICommand ViewDetailsCommand => _viewDetailCommand ??=
            new AsyncCommand<PostItem>(async (item) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Selected = item;
                });

                _detailViewModel.Selected = Items.First(x => x.Id == item.Id);
                await _navigation.PushAsync(new Views.PostDetailPage(_detailViewModel), true);
                var feedback = await PostItemService.GetPostFeedbackAsync(item.Id);
                _detailViewModel.Selected.FeedBack.ReplaceRange(feedback.Data);
            });

        public ICommand LikeCommand => _likeCommand ??= new AsyncCommand<PostItem>(async (item) =>
        {
            var it = Items.First(x => x.Id == item.Id);
            await PostItemService.SetUserReaction(it);
        });

        public ObservableRangeCollection<PostItem> Items { get; } = new ObservableRangeCollection<PostItem>();
        public ObservableRangeCollection<CategoryModel> Categories { get; } = new ObservableRangeCollection<CategoryModel>();
        public PostItem Selected
        {
            get => selected; 
            set
            {
                SetProperty(ref selected, value);
            }
        }

        public IPostItemService PostItemService { get; }
        private CompositeDisposable _disposables = new CompositeDisposable();
        public PostsViewModel(INavigationService navigation, IPostItemService postItemService, PostDetailViewModel detailViewModel)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            _detailViewModel = detailViewModel ?? throw new ArgumentNullException(nameof(detailViewModel));
            
            this.WhenAnyValue(x => x.IsBusy)
                .Where(busy => busy)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(async o =>
                {
                    var posts = await PostItemService.GetItemsAsync();
                    if (!posts.HasError)
                    {
                        Items.ReplaceRange(posts.Data);
                    }
                    IsBusy = false;
                })
                .Do(async o => {
                    var categories = await PostItemService.GetCategoriesAsync();
                    if (!categories.HasError)
                        Categories.ReplaceRange(categories.Data);
                })
                .DelaySubscription(TimeSpan.FromSeconds(5))
                .Subscribe()
                .DisposeWith(_disposables);
            IsBusy = true;
            Xamarin.Forms.MessagingCenter.Subscribe<FavoriteViewModel, PostItem>(this, "favorite", (fvm, item) =>
            {
                var it = Items.FirstOrDefault(x => x.Id == item.Id);
                if (it != null)
                    it.Favorite = item.Favorite;
            });
            Xamarin.Forms.MessagingCenter.Subscribe<PostDetailViewModel, PostItem>(this, "favorite", (fvm, item) =>
            {
                var it = Items.FirstOrDefault(x => x.Id == item.Id);
                if (it != null && it != Selected)
                    it.Favorite = item.Favorite;
            });
        }
    }
}
