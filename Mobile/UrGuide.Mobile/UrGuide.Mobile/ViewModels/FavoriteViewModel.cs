using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.ViewModels
{
    class FavoriteViewModel
    : BaseViewModel
    {
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;
        private PostItem selected;
        private readonly INavigationService _navigation;

        private readonly PostDetailViewModel _detailViewModel;
        private ICommand _viewDetailCommand;
        private ICommand _likeCommand;
        private ICommand _dislikeCommand;
        private ICommand _removeFavoriteCommand;
        private ICommand _loadItemsCommand;

        public ICommand LoadItemsCommand => _loadItemsCommand ??= new AsyncCommand(async () =>
        {
            IsBusy = true;
            var items = await PostItemService.GetFavoriteAsync();
            Items.ReplaceRange(items);
            IsBusy = false;
        });

        public ICommand RemoveFavoriteCommand => _removeFavoriteCommand ??= new AsyncCommand<PostItem>(async (item) =>
        {
            var it = Items.First(x => x.Id == item.Id);
            it.Favorite = false;
            await PostItemService.ToggleFavorites(it);
            Items.Remove(it);
            Xamarin.Forms.MessagingCenter.Send(this, "favorite", it);
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

        public ObservableRangeCollection<PostItem> Items { get; }
        public PostItem Selected
        {
            get => selected;
            set
            {
                SetProperty(ref selected, value);
            }
        }

        public IPostItemService PostItemService { get; }

        public FavoriteViewModel(INavigationService navigation, PostDetailViewModel detailViewModel, IPostItemService postItemService)
        {
            Items = new ObservableRangeCollection<PostItem>();
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _detailViewModel = detailViewModel ?? throw new ArgumentNullException(nameof(detailViewModel));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
        }
    }
}
