using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Views.Dialog;

namespace UrGuide.Mobile.ViewModels
{
    public class PostDetailViewModel : BaseViewModel
    {
        private PostItem selected;

        private ICommand _likeCommand;
        private ICommand _dislikeCommand;
        private ICommand _viewBidCommand;

        public ICommand ViewBidCommand => _viewBidCommand ??= new AsyncCommand<PostItem>(async (item) =>
        {
            BidDialogViewModel.Item = item;
            await NavigationService.PushModalAsync(new BidDialog(BidDialogViewModel), true);
        });

        public ICommand LikeCommand => _likeCommand ??= new Command(() =>
        {
            var it = Selected;
            if (it.ReactionType == PostsViewModel.Like)
            {
                it.Likes--;
                it.ReactionType = PostsViewModel.Unknown;
                return;
            }
            if (it.ReactionType == PostsViewModel.DisLike)
            {
                it.Dislikes--;
            }
            it.Likes++;
            it.ReactionType = PostsViewModel.Like;
        });

        public ICommand DislikeCommand => _dislikeCommand ??= new Command(() =>
        {
            var it = Selected;
            if (it.ReactionType == PostsViewModel.DisLike)
            {
                it.Dislikes--;
                it.ReactionType = PostsViewModel.Unknown;
                return;
            }
            if (it.ReactionType == PostsViewModel.Like)
            {
                it.Likes--;
            }
            it.Dislikes++;
            it.ReactionType = PostsViewModel.DisLike;
        });

        public PostItem Selected
        {
            get => selected; set
            {
                SetProperty(ref selected, value);
            }
        }

        public INavigationService NavigationService { get; }
        public BidDialogViewModel BidDialogViewModel { get; }

        public PostDetailViewModel(INavigationService navigationService, BidDialogViewModel bidDialogViewModel)
        {
            NavigationService = navigationService ?? throw new System.ArgumentNullException(nameof(navigationService));
            BidDialogViewModel = bidDialogViewModel ?? throw new System.ArgumentNullException(nameof(bidDialogViewModel));
        }
    }
}
