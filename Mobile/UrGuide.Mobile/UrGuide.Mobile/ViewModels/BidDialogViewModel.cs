using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
using UrGuide.Model.Posts;

namespace UrGuide.Mobile.ViewModels
{
    public class BidDialogViewModel : BaseViewModel
    {
        private ICommand _closeDialogCommand;
        private ICommand _loadBidCommand;
        private ICommand _newBidCommand;
        private PostItem item;
        private bool canApproveBid = true;
        private bool canRejectBid = true;

        public BidDialogViewModel(INavigationService navigationService,
            IPostItemService postItemService,
            IPreferenceService preference)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            Preference = preference ?? throw new ArgumentNullException(nameof(preference));

            RejectBidCommand = new Command<BidHistoryModel>(async i =>
            {
                var r = await PostItemService.RejectBid(Item.Id).ConfigureAwait(true);
                if (!r.HasError)
                {
                    i.IsActive = false;
                    this.OnPropertyChanged(nameof(Items));
                }
            }, x => CanRejectBid);
            AcceptBidCommand = new Command<BidHistoryModel>(async i =>
            {
                var r = await PostItemService.AcceptBid(Item.Id).ConfigureAwait(true);
                if (!r.HasError)
                {
                    i.IsActive = false;
                    this.OnPropertyChanged(nameof(Items));
                }
            }, x => CanApproveBid);
        }
        public PostItem Item
        {
            get => item; set
            {
                item = value;
                LoadBidCommand.Execute(null);
            }
        }
        public ObservableRangeCollection<BidHistoryModel> Items { get; } = new ObservableRangeCollection<BidHistoryModel>();
        public bool CanRejectBid { get => canRejectBid; set => SetProperty(ref canRejectBid, value); }
        public bool CanApproveBid { get => canApproveBid; set => SetProperty(ref canApproveBid, value); }
        public INavigationService NavigationService { get; }
        public IPostItemService PostItemService { get; }
        public IPreferenceService Preference { get; }
        public string PostId { get; set; }

        public ICommand AcceptBidCommand { get; }
        public ICommand RejectBidCommand { get; }
        public ICommand CloseDialogCommand => _closeDialogCommand ??= new AsyncCommand(async () =>
        {
            await NavigationService.PopModalAsync();
        });

        public ICommand LoadBidCommand => _loadBidCommand ??= new AsyncCommand(async () =>
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = true;
            });
            var items = await PostItemService.GetBidHistoryAsync(Item.Id);
            Items.ReplaceRange(items.Data);
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                IsBusy = false;
                CanRejectBid = Preference.UserId == Item.AuthorId && Item.Countdown.IsActive;
                CanApproveBid = CanRejectBid;
            });

        });

        public ICommand NewBidCommand => _newBidCommand ??= new Command<double>(async (bid) =>
        {
            var item = await PostItemService.Bid(Item.Id, bid);
            if (!item.HasError)
            {
                Items.Add(new BidHistoryModel
                {
                    Author = Preference.FullName,
                    AuthorImage = $"{GlobalSetting.DefaultEndpoint}/{Preference.Image}",
                    Created = DateTime.Now.ToString("dd-MMM-yyyy HH:mm"),
                    Value = $"${bid:#}"
                });
            }
        });
    }
}
