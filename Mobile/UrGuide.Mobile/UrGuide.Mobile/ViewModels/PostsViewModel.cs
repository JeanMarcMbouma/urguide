using MvvmHelpers.Commands;
using Plugin.SharedTransitions;
using ReactiveUI;
using Sharpnado.Presentation.Forms;
using Sharpnado.Presentation.Forms.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Lookup;

namespace UrGuide.Mobile.ViewModels
{
    public class PostsViewModel : ReactiveObject, INavigatableViewModel
    {
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;
        private PostItem selected;
        private readonly INavigationService _navigation;
        private readonly PostDetailViewModel _detailViewModel;
        private ICommand _viewDetailCommand;
        private ICommand _likeCommand;
        private ICommand _markAsFavoriteCommand;
        private ICommand _searchCategoryCommand;

        public TaskLoaderNotifier<IEnumerable<PostItem>> PostItemsLoader { get; }
        public TaskLoaderNotifier<IEnumerable<CategoryModel>> CategoryLoader { get; }
        public Paginator<IEnumerable<PostItem>> Paginator { get; }

        public ICommand ToggleFavoriteCommand => _markAsFavoriteCommand ??= new AsyncCommand<PostItem>(async item =>
        {
            var it = PostItemsLoader.Result.First(x => x.Id == item.Id);
            it.Favorite = !it.Favorite;
            await PostItemService.ToggleFavorites(it);
        });

        public ICommand ViewDetailsCommand => _viewDetailCommand ??=
            new AsyncCommand<PostItem>(async (item) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Selected = item;
                });
                
                _detailViewModel.Selected = PostItemsLoader.Result.First(x => x.Id == item.Id);
                await _navigation.PushAsyncWithSharedTransition(new Views.PostDetailPage(_detailViewModel), item.Id);
            });

        public ICommand LikeCommand => _likeCommand ??= new AsyncCommand<PostItem>(async (item) =>
        {
            var it = PostItemsLoader.Result.First(x => x.Id == item.Id);
            await PostItemService.SetUserReaction(it);
        });

        public PostItem Selected
        {
            get => selected; 
            set
            {
                this.RaiseAndSetIfChanged(ref selected, value);
            }
        }

        public IPostItemService PostItemService { get; }
        public PostsViewModel(INavigationService navigation, IPostItemService postItemService, PostDetailViewModel detailViewModel)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            _detailViewModel = detailViewModel ?? throw new ArgumentNullException(nameof(detailViewModel));
            PostItemsLoader = new TaskLoaderNotifier<IEnumerable<PostItem>>(LoadItemsAsync);
            CategoryLoader = new TaskLoaderNotifier<IEnumerable<CategoryModel>>(LoadCategoriesAsync);

            Xamarin.Forms.MessagingCenter.Subscribe<FavoriteViewModel, PostItem>(this, "favorite", (fvm, item) =>
            {
                var it = PostItemsLoader.Result.FirstOrDefault(x => x.Id == item.Id);
                if (it != null)
                    it.Favorite = item.Favorite;
            });
            Xamarin.Forms.MessagingCenter.Subscribe<PostDetailViewModel, PostItem>(this, "favorite", (fvm, item) =>
            {
                var it = PostItemsLoader.Result.FirstOrDefault(x => x.Id == item.Id);
                if (it != null && it != Selected)
                    it.Favorite = item.Favorite;
            });
        }

        private async Task<IEnumerable<CategoryModel>> LoadCategoriesAsync()
        {
            var categories = await PostItemService.GetCategoriesAsync();
            return categories.Data;
        }

        private async Task<IEnumerable<PostItem>> LoadItemsAsync()
        {
            var posts = await PostItemService.GetItemsAsync();
            return posts.Data;
        }

        public void Load(object parameter)
        {
            CategoryLoader.Load();
            PostItemsLoader.Load();
        }
    }
}
