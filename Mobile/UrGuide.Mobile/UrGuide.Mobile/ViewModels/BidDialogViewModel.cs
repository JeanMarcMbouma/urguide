using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
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
        public BidDialogViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService ?? throw new System.ArgumentNullException(nameof(navigationService));
            Items = new ObservableRangeCollection<BidHistoryModel>
            {
                new BidHistoryModel
                {
                    Author = "John Doe",
                    AuthorImage = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
                    Created = "12-Jun-2020 12:30",
                    Value = "$45"
                },
                new BidHistoryModel
                {
                    Author = "Jane Doe",
                    AuthorImage = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
                    Created = "12-Jun-2020 12:30",
                    Value = "$45"
                },
                new BidHistoryModel
                {
                    Author = "Guffy",
                    AuthorImage = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
                    Created = "12-Jun-2020 13:30",
                    Value = "$45",
                    IsActive = true
                }
            };
        }
        public PostItem Item { get; set; }
        public ObservableRangeCollection<BidHistoryModel> Items { get; }
        public bool CanRejectBid { get; set; } = true;
        public bool CanApproveBid { get; set; } = true;
        public INavigationService NavigationService { get; }

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
            await Task.Delay(3000);
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
