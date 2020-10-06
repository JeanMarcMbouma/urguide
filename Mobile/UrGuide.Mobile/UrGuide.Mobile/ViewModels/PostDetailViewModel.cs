using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
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
        public ICommand ToggleFavoriteCommand => _markAsFavoriteCommand ??= new AsyncCommand(async () =>
        {
            Selected.Favorite = !Selected.Favorite;
            await PostItemService.ToggleFavorites(Selected);
            Xamarin.Forms.MessagingCenter.Send(this, "favorite", Selected);
        });
        public ICommand NewFeedBackCommand => _newFeedbackCommand ??= new Command(async () =>
        {
            if (!string.IsNullOrEmpty(NewFeedBack.Text))
            {
                var it = await PostItemService.SendFeedback(Selected.Id, NewFeedBack);
                if(!it.HasError)
                    Selected.FeedBack.Add(it.Data);
            }
            NewFeedBack.Rating = 4;
            NewFeedBack.Text = string.Empty;
            OnPropertyChanged(nameof(NewFeedBack));
        }, () => (NewFeedBack.Text ?? string.Empty).Length > 50);
        public ICommand ViewBidCommand => _viewBidCommand ??= new AsyncCommand<PostItem>(async (item) =>
        {
            BidDialogViewModel.Item = item;
            await NavigationService.PushModalAsync(new BidDialog(BidDialogViewModel), true);
        });

        public ICommand LikeCommand => _likeCommand ??= new Command(async () =>
        {
            await PostItemService.SetUserReaction(Selected);
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
            Rating = 4
        };
        public INavigationService NavigationService { get; }
        public IPostItemService PostItemService { get; }
        public BidDialogViewModel BidDialogViewModel { get; }
        public IPreferenceService Preference { get; }

        private string _id;
        public string Id { 
            get { return _id; }
            set {
                _id = value;
                LoadSelectedItemCommand.Execute(null);
            } 
        }

        public bool CanReview => Selected?.AuthorId != Preference.UserId
            && Selected?.FeedBack.All(f => f.AuthorId != Preference.UserId) == true;

        public PostDetailViewModel(INavigationService navigationService, 
            IPostItemService postItemService, 
            BidDialogViewModel bidDialogViewModel,
            IPreferenceService preference)
        {
            NavigationService = navigationService ?? throw new System.ArgumentNullException(nameof(navigationService));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            BidDialogViewModel = bidDialogViewModel ?? throw new System.ArgumentNullException(nameof(bidDialogViewModel));
            Preference = preference ?? throw new ArgumentNullException(nameof(preference));
        }
    }
}
