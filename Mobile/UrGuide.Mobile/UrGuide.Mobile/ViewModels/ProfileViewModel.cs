using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Services;
using UrGuide.Mobile.Views.Dialog;
using UrGuide.Model.Shared;
using UrGuide.Model.Users;

namespace UrGuide.Mobile.ViewModels
{
    class ProfileViewModel : BaseViewModel, INavigatableViewModel
    {
        private ProfileDisplayMode mode = ProfileDisplayMode.Reviews;
        private ICommand _viewReviewsCommand;
        private ICommand _viewPostsCommand;
        private ICommand _viewGaleryCommand;
        private ICommand _viewPostDetailsCommand;
        private ICommand _loadItemsCommand;
        private ICommand _editProfileCommand;
        private ICommand _createGalleryCommand;
        private ICommand _changePasswordCommand;
        private ICommand _deleteAccountCommand;
        private UserInfo userInfo;

        public ProfileViewModel(INavigationService navigation, IPostItemService postItemService, IUserService userService)
        {
            Navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public UserInfo UserInfo { get => userInfo; set => SetProperty(ref userInfo, value); }

        public ProfileDisplayMode Mode
        {
            get => mode; set
            {
                SetProperty(ref mode, value);
                Items.Replace(new WrapperViewModel
                {
                    Mode = value
                });
            }
        }

        public ObservableRangeCollection<WrapperViewModel> Items { get; set; } = new ObservableRangeCollection<WrapperViewModel>
        {
            new WrapperViewModel()
        };

        public ICommand EditProfileCommand => _editProfileCommand ??= new AsyncCommand(async () => await Navigation.PushModalAsync(new EditProfile()));
        public ICommand ChangePasswordCommand => _changePasswordCommand ??= new AsyncCommand(async () => await Navigation.PushModalAsync(new ChangePassword()));
        public ICommand DeleteAccountCommand => _deleteAccountCommand ??= new AsyncCommand(async () => await Navigation.ConfirmAsync(DeleteAccount));

        private void DeleteAccount(DialogResult result)
        {
            
        }

        public ICommand CreateGalleryCommand => _createGalleryCommand ??= new AsyncCommand(async () => await Navigation.PushModalAsync(new CreateGallery()));
        public ICommand LoadItemsCommand => _loadItemsCommand ??= new AsyncCommand(async () =>
        {
            IsBusy = true;
            var items = await PostItemService.GetItemsAsync();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                if(!items.HasError)
                    Posts.ReplaceRange(items.Data);
                UserInfo = UserService.GetUserInfo(null);
                IsBusy = false;
            });
        });
        public ICommand ViewReviewsCommand => _viewReviewsCommand ??= new Command(() => Mode = ProfileDisplayMode.Reviews);
        public ICommand ViewPostsCommand => _viewPostsCommand ??= new Command(() => Mode = ProfileDisplayMode.Posts);
        public ICommand ViewGalleryCommand => _viewGaleryCommand ??= new Command(() => Mode = ProfileDisplayMode.Gallery);
        public ObservableRangeCollection<AuthoredFeedback> Feedbacks { get; set; } 
            = new ObservableRangeCollection<AuthoredFeedback>();
        public ObservableRangeCollection<GalleryItem> Catalogs { get; set; } 
            = new ObservableRangeCollection<GalleryItem>();
        public ObservableRangeCollection<PostItem> Posts { get; set; } = new ObservableRangeCollection<PostItem>();
        public FeedbackModel NewFeedBack { get; } = new FeedbackModel
        {
            Rating = 1
        };
        public INavigationService Navigation { get; }
        public IPostItemService PostItemService { get; }
        public IUserService UserService { get; }

        public class WrapperViewModel : BaseViewModel
        {
            private ProfileDisplayMode mode = ProfileDisplayMode.Reviews;

            public ProfileDisplayMode Mode { get => mode; set => SetProperty(ref mode, value); }

        }
        public void Load(object paramter) { }

    }
    public enum ProfileDisplayMode
    {
        Reviews = 1,
        Posts = 2,
        Gallery = 3
    }
}
