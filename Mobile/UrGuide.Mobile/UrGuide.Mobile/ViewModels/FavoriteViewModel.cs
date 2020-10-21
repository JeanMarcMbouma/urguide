using MvvmHelpers;
using MvvmHelpers.Commands;
using Sharpnado.Presentation.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.ViewModels
{
    class FavoriteViewModel
    : BaseViewModel, INavigatableViewModel
    {
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;
        private PostItem selected;
        private readonly INavigationService _navigation;

        private readonly PostDetailViewModel _detailViewModel;
        private ICommand _viewDetailCommand;
        private ICommand _likeCommand;
        private ICommand _removeFavoriteCommand;

        public ICommand RemoveFavoriteCommand => _removeFavoriteCommand ??= new AsyncCommand<PostItem>(async (item) =>
        {
            var it = ItemsLoader.Result.First(x => x.Id == item.Id);
            it.Favorite = false;
            await PostItemService.ToggleFavorites(it);
            ItemsLoader.Load();
            Xamarin.Forms.MessagingCenter.Send(this, "favorite", it);
        });
        public ICommand ViewDetailsCommand => _viewDetailCommand ??=
            new AsyncCommand<PostItem>(async (item) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Selected = item;
                });

                _detailViewModel.Selected = ItemsLoader.Result.First(x => x.Id == item.Id);
                await _navigation.PushAsyncWithSharedTransition(new Views.PostDetailPage(_detailViewModel), item.Id);
            });

        public ICommand LikeCommand => _likeCommand ??= new Command<PostItem>(async (item) =>
        {
            var it = ItemsLoader.Result.First(x => x.Id == item.Id);
            await PostItemService.SetUserReaction(it);
        });

        public TaskLoaderNotifier<IEnumerable<PostItem>> ItemsLoader { get; }
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
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            _detailViewModel = detailViewModel ?? throw new ArgumentNullException(nameof(detailViewModel));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            ItemsLoader = new TaskLoaderNotifier<IEnumerable<PostItem>>(PostItemService.GetFavoriteAsync);
        }
        public void Load(object paramter) => ItemsLoader.Load();
    }
}
