using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Posts;

namespace UrGuide.Mobile.ViewModels
{
    public class BidDialogViewModel : BaseViewModel
    {
        private ICommand _closeDialogCommand;
        private ICommand _loadBidCommand;
        private ICommand _newBidCommand;
        private PostItem item;

        public BidDialogViewModel(INavigationService navigationService, IPostItemService postItemService)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
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
        public bool CanRejectBid { get; set; } = true;
        public bool CanApproveBid { get; set; } = true;
        public INavigationService NavigationService { get; }
        public IPostItemService PostItemService { get; }

        public string PostId { get; set; }
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
            });
        });

        public ICommand NewBidCommand => _newBidCommand ??= new Command<double>((bid) =>
        {
            Items.Add(new BidHistoryModel
            {
                Author = "Me",
                AuthorImage = "http://urguide.azurewebsites.net/images/00000000-0000-0000-0000-0000000000.png",
                Created = DateTime.Now.ToString("dd-MMM-yyyy HH:mm"),
                Value = $"${bid:#}"
            });
        });
    }
}
