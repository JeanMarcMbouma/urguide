using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        private ICommand _dislikeCommand;
        private ICommand _markAsFavoriteCommand;
        private ICommand _loadItemsCommand;
        private ICommand _searchCategoryCommand;

        public ICommand SearchCategoryCommand => _searchCategoryCommand ??= new AsyncCommand<CategoryModel>(async (model) => await _navigation.GotoAsync($"//discover?Category={model.Name}"));
        public ICommand ToggleFavoriteCommand => _markAsFavoriteCommand ??= new AsyncCommand<PostItem>(async item =>
        {
            var it = Items.First(x => x.Id == item.Id);
            it.Favorite = !it.Favorite;
            await PostItemService.ToggleFavorites(it);
        });

        public ICommand LoadItemsCommand => _loadItemsCommand ??= new AsyncCommand(async () =>
        {
            var items = await PostItemService.GetItemsAsync().ConfigureAwait(false);
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if(!items.HasError)
                    Items.ReplaceRange(items.Data);
                IsBusy = false;
            });
        });

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

        public ICommand LikeCommand => _likeCommand ??= new Command<PostItem>((item) =>
        {
            var it = Items.First(x => x.Id == item.Id);
            if (it.ReactionType == Like)
            {
                it.Likes--;
                it.ReactionType = Unknown;
                return;
            }
            if (it.ReactionType == DisLike)
            {
                it.Dislikes--;
            }
            it.Likes++;
            it.ReactionType = Like;
        });

        public ICommand DislikeCommand => _dislikeCommand ??= new Command<PostItem>((item) =>
        {
            var it = Items.First(x => x.Id == item.Id);
            if (it.ReactionType == DisLike)
            { 
                it.Dislikes--;
                it.ReactionType = Unknown;
                return;
            }
            if (it.ReactionType == Like)
            { 
                it.Likes--; 
            }
            it.Dislikes++;
            it.ReactionType = DisLike;
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

        public PostsViewModel(INavigationService navigation, IPostItemService postItemService, PostDetailViewModel detailViewModel)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            _detailViewModel = detailViewModel ?? throw new ArgumentNullException(nameof(detailViewModel));
            IsBusy = true;
        }

        public async Task Init()
        {
            if (_initialized)
                return;
            var r = await PostItemService.GetCategoriesAsync().ConfigureAwait(false);
            if (!r.HasError)
            {
                _initialized = true;
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Categories.ReplaceRange(r.Data);
                });
            }
            LoadItemsCommand.Execute(null);
        }
    }
}
