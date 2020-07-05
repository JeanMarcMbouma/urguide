using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Windows.Input;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.ViewModels
{
    public class PostDetailViewModel : BaseViewModel
    {
        private PostItem selected;

        private ICommand _likeCommand;
        private ICommand _dislikeCommand;

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
        public PostDetailViewModel(PostItem item)
        {
            Selected = item ?? throw new System.ArgumentNullException(nameof(item));
        }
    }
}
