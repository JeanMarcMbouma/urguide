using MvvmHelpers;
using MvvmHelpers.Commands;
using ReactiveUI;
using Sharpnado.Presentation.Forms;
using Sharpnado.Presentation.Forms.Paging;
using Sharpnado.Presentation.Forms.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.Views.Dialog;
using Xamarin.Essentials;

namespace UrGuide.Mobile.ViewModels
{
    public class PostDetailViewModel : ReactiveObject, INavigatableViewModel
    {
        private PostItem selected;

        private ICommand _likeCommand;
        private ICommand _viewBidCommand;
        private ICommand _markAsFavoriteCommand;
        public TaskLoaderNotifier LoadSelectedItem { get; }
        public ICommand ToggleFavoriteCommand => _markAsFavoriteCommand ??= new AsyncCommand(async () =>
        {
            Selected.Favorite = !Selected.Favorite;
            await PostItemService.ToggleFavorites(Selected);
            Xamarin.Forms.MessagingCenter.Send(this, "favorite", Selected);
        });
        public ReactiveCommand<Unit, Task> NewFeedBackCommand { get; }
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
                this.RaiseAndSetIfChanged(ref selected, value);
                Load();
            }
        }

        public TaskLoaderNotifier<IEnumerable<Model.Shared.AuthoredFeedback>> FeedbacksLoader { get; }
        public Paginator<Model.Shared.AuthoredFeedback> FeedbackPaginator { get; }
        public ObservableRangeCollection<Model.Shared.AuthoredFeedback> Feedbacks { get; set; }
            = new ObservableRangeCollection<Model.Shared.AuthoredFeedback>();

        public INavigationService NavigationService { get; }
        public IPostItemService PostItemService { get; }
        public BidDialogViewModel BidDialogViewModel { get; }
        public IPreferenceService Preference { get; }

        private string _id;
        private bool isLoggedIn;
        private bool canReview;
        private int newFeedbackRating = 1;
        private string newFeedbackText = string.Empty;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                LoadSelectedItem.Load();
            }
        }

        public bool CanReview { get => canReview; set => this.RaiseAndSetIfChanged(ref canReview, value); }

        public bool IsLoggedIn { get => isLoggedIn; set => this.RaiseAndSetIfChanged(ref isLoggedIn, value); }
        public ICommand MakeReservationCommand { get; }
        public ICommand SharePostCommand { get; }
        public string NewFeedbackText { get => newFeedbackText; set => this.RaiseAndSetIfChanged(ref newFeedbackText,  value); }
        public int NewFeedbackRating { get => newFeedbackRating; set => this.RaiseAndSetIfChanged(ref newFeedbackRating, value); }
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
            LoadSelectedItem = new TaskLoaderNotifier(async () =>
            {
                await PostItemService.GetByIdAsync(_id).ContinueWith(r =>
                {
                    var result = r.Result;
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                    {

                        if (!result.HasError)
                            Selected = result.Data;
                    });
                    return r.Result;
                });
                Load();
            });
            this.WhenAnyValue(x => x.Preference.UserId, x => x.Preference.Role, (u, r) =>
            {
                return (IsLoggedIn: !string.IsNullOrEmpty(u), IsGuide: "guide".Equals(r, StringComparison.OrdinalIgnoreCase));
            })
            .Do((data) =>
            {
                IsLoggedIn = data.IsLoggedIn;
            }).Subscribe();

            this.WhenAnyValue(x => x.Feedbacks, x => x.Selected)
                .Do((x) =>
                {
                    CanReview = IsLoggedIn &&
                    x.Item2?.AuthorId != Preference.UserId &&
                    (!x.Item1.Any() || x.Item1.All(f => f.AuthorId != Preference.UserId));
                }).Subscribe();
            var canSubmitReview = this.WhenAnyValue(x => x.NewFeedbackRating, x => x.NewFeedbackText)
                .Select((x) => x.Item2?.Length >= 50);

            NewFeedBackCommand = ReactiveCommand.Create(async () =>
            {
                var it = await PostItemService.SendFeedback(Selected.Id, new Model.Shared.FeedbackModel
                {
                    Rating = NewFeedbackRating,
                    Text = NewFeedbackText
                });
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!it.HasError)
                        Load();
                    NewFeedbackRating = 1;
                    NewFeedbackText = string.Empty;
                });
            }, canSubmitReview, RxApp.MainThreadScheduler);
            NewFeedBackCommand.Subscribe();

            SharePostCommand = new Command<PostItem>(async it =>
            {
                await PostItemService.ShareItem(it);
            });

            MakeReservationCommand = new Command<PostItem>(async it =>
            {
                await PostItemService.ToggleReservation(it).ConfigureAwait(true);
            }, it => IsLoggedIn);
        }

        private async Task<PageResult<Model.Shared.AuthoredFeedback>> LoadFeedbackPageAsync(int pageNumber, int pageSize, bool arg3)
        {
            var result = await PostItemService.GetPostFeedbackAsync(Id, pageNumber);
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
            FeedbacksLoader.Load(async () => (await FeedbackPaginator.LoadPage(1)).Items);
            this.RaisePropertyChanged(nameof(Feedbacks));
        }
    }
}
