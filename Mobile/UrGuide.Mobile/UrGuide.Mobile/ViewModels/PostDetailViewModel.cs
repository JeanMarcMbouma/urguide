using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
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
        private ICommand _newFeedbackCommand;
        private ICommand _markAsFavoriteCommand;
        private ICommand _loadSelectedItemCommand;

        public ICommand LoadSelectedItemCommand => _loadSelectedItemCommand ??= new AsyncCommand(async () =>
        {
            IsBusy = true;
            await PostItemService.GetByIdAsync(_id).ContinueWith(r => {
                var result = r.Result;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {

                    if (!result.HasError)
                        Selected = result.Data;
                    IsBusy = false;
                });
                return r.Result;
            });
        });
        public ICommand ToggleFavoriteCommand => _markAsFavoriteCommand ??= new Command(() =>
        {
            Selected.Favorite = !Selected.Favorite;
        });
        public ICommand NewFeedBackCommand => _newFeedbackCommand ??= new Command(() =>
        {
            if (!string.IsNullOrEmpty(NewFeedBack.Text))
            {
                Selected.FeedBack.Add(new Model.Shared.AuthoredFeedback
                {
                    AuthorFullName = "Me",
                    AuthorImage = "http://urguide.azurewebsites.net/thumb/00000000-0000-0000-0000-000000000000.png",
                    PublicationDate = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"),
                    Rating = NewFeedBack.Rating,
                    Text = NewFeedBack.Text
                });
            }
            NewFeedBack.Rating = 1;
            NewFeedBack.Text = string.Empty;
            OnPropertyChanged(nameof(NewFeedBack));
        });
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
                _id = value.Id;
                SetProperty(ref selected, value);
            }
        }

        public UrGuide.Model.Shared.FeedbackModel NewFeedBack { get; } = new Model.Shared.FeedbackModel { 
            Rating = 1
        };
        public INavigationService NavigationService { get; }
        public IPostItemService PostItemService { get; }
        public BidDialogViewModel BidDialogViewModel { get; }

        private string _id;
        public string Id { 
            get { return _id; }
            set {
                _id = value;
                LoadSelectedItemCommand.Execute(null);
            } 
        }

        public PostDetailViewModel(INavigationService navigationService, IPostItemService postItemService, BidDialogViewModel bidDialogViewModel)
        {
            NavigationService = navigationService ?? throw new System.ArgumentNullException(nameof(navigationService));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            BidDialogViewModel = bidDialogViewModel ?? throw new System.ArgumentNullException(nameof(bidDialogViewModel));
        }
    }
}
