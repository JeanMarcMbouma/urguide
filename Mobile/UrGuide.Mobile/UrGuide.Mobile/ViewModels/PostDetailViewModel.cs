using MvvmHelpers;
using MvvmHelpers.Commands;
using Sharpnado.Presentation.Forms;
using Sharpnado.Presentation.Forms.Paging;
using Sharpnado.Presentation.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UrGuide.Mobile.API;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.Views.Dialog;

namespace UrGuide.Mobile.ViewModels
{
    public class PostDetailViewModel : BaseViewModel, INavigatableViewModel
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
            Load();
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
                Load();
            }
        }

        public TaskLoaderNotifier<IEnumerable<Model.Shared.AuthoredFeedback>> FeedbacksLoader { get; }
        public Paginator<Model.Shared.AuthoredFeedback> FeedbackPaginator { get; }
        public ObservableRangeCollection<Model.Shared.AuthoredFeedback> Feedbacks { get; set; }

        public Model.Shared.FeedbackModel NewFeedBack { get; } = new Model.Shared.FeedbackModel { 
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
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            BidDialogViewModel = bidDialogViewModel ?? throw new ArgumentNullException(nameof(bidDialogViewModel));
            Preference = preference ?? throw new ArgumentNullException(nameof(preference));
            FeedbacksLoader = new TaskLoaderNotifier<IEnumerable<Model.Shared.AuthoredFeedback>>();
            FeedbackPaginator = new Paginator<Model.Shared.AuthoredFeedback>(LoadFeedbackPageAsync);
        }

        private async Task<PageResult<Model.Shared.AuthoredFeedback>> LoadFeedbackPageAsync(int pageSize, int pageNumber, bool arg3)
        {
            var result =  await PostItemService.GetPostFeedbackAsync(Id, pageNumber);
            Feedbacks.AddRange(result.Items);
            return result;
        }

        public void Load(object parameter)
        {
            Load();
        }

        private void Load()
        {
            Feedbacks = new ObservableRangeCollection<Model.Shared.AuthoredFeedback>();
            OnPropertyChanged(nameof(Feedbacks));
            FeedbacksLoader.Load(async () => (await FeedbackPaginator.LoadPage(1)).Items);
        }
    }
}
