using MvvmHelpers;
using MvvmHelpers.Commands;
using Sharpnado.Presentation.Forms;
using Sharpnado.Presentation.Forms.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
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
        private ICommand _editProfileCommand;
        private ICommand _createGalleryCommand;
        private ICommand _changePasswordCommand;
        private ICommand _deleteAccountCommand;

        public ProfileViewModel(INavigationService navigation, 
            IPostItemService postItemService, 
            IUserService userService,
            IPreferenceService preference)
        {
            Navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            Preference = preference ?? throw new ArgumentNullException(nameof(preference));
            UserLoader = new TaskLoaderNotifier<UserInfo>(LoadUserAsync);
            PostsLoader = new TaskLoaderNotifier<IEnumerable<PostItem>>();
            GalleryLoader = new TaskLoaderNotifier<IEnumerable<GalleryItem>>(async () =>
            {
                var items = await UserService.GetGalleryItems(Preference.UserId);
                return items;
            });
            FeedbackLoader = new TaskLoaderNotifier<IEnumerable<AuthoredFeedback>>();
            PostsPaginator = new Paginator<PostItem>(async (pageNumber, pageSize, b) =>
            {
                var result = await PostItemService.GetUserPosts(Preference.UserId, pageNumber);
                Posts ??= new ObservableRangeCollection<PostItem>();
                Posts.AddRange(result.Items);
                return result;
            });
            FeedbackPaginator = new Paginator<AuthoredFeedback>(async (pageNumber, pageSize, b) =>
            {
                var result = await UserService.GetUserFeedback(Preference.UserId, pageNumber);
                Feedbacks ??= new ObservableRangeCollection<AuthoredFeedback>();
                Feedbacks.AddRange(result.Items);
                return result;
            });
        }

        private Task<UserInfo> LoadUserAsync()
        {
            return UserService.GetUserInfo(Preference.UserId);
        }


        public ProfileDisplayMode Mode
        {
            get => mode; set
            {
                SetProperty(ref mode, value);
                Items.Replace(new WrapperViewModel
                {
                    Mode = value
                });
                switch (value)
                {
                    case ProfileDisplayMode.Reviews:
                        FeedbackLoader.Load();
                        break;
                    case ProfileDisplayMode.Posts:
                        PostsLoader.Load();
                        break;
                    case ProfileDisplayMode.Gallery:
                        GalleryLoader.Load();
                        break;
                    default:
                        break;
                }
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
       
        public ICommand ViewReviewsCommand => _viewReviewsCommand ??= new Command(() => Mode = ProfileDisplayMode.Reviews);
        public ICommand ViewPostsCommand => _viewPostsCommand ??= new Command(() => Mode = ProfileDisplayMode.Posts);
        public ICommand ViewGalleryCommand => _viewGaleryCommand ??= new Command(() => Mode = ProfileDisplayMode.Gallery);
        public ObservableRangeCollection<AuthoredFeedback> Feedbacks { get; set; } 
            = new ObservableRangeCollection<AuthoredFeedback>();
        public ObservableRangeCollection<PostItem> Posts { get; set; } = new ObservableRangeCollection<PostItem>();
        public FeedbackModel NewFeedBack { get; } = new FeedbackModel
        {
            Rating = 1
        };
        public INavigationService Navigation { get; }
        public IPostItemService PostItemService { get; }
        public IUserService UserService { get; }
        public IPreferenceService Preference { get; }

        public TaskLoaderNotifier<UserInfo> UserLoader { get; }
        public TaskLoaderNotifier<IEnumerable<AuthoredFeedback>> FeedbackLoader { get; }
        public TaskLoaderNotifier<IEnumerable<PostItem>> PostsLoader { get; }
        public TaskLoaderNotifier<IEnumerable<GalleryItem>> GalleryLoader { get; }
        public Paginator<PostItem> PostsPaginator { get; }
        public Paginator<AuthoredFeedback> FeedbackPaginator { get; }

        public class WrapperViewModel : BaseViewModel
        {
            private ProfileDisplayMode mode = ProfileDisplayMode.Reviews;

            public ProfileDisplayMode Mode { get => mode; set => SetProperty(ref mode, value); }

        }
        public void Load(object paramter) {
            UserLoader.Load();
            PostsLoader.Load(async () => (await PostsPaginator.LoadPage(1)).Items);
            FeedbackLoader.Load(async () => (await FeedbackPaginator.LoadPage(1)).Items);
            GalleryLoader.Load();
        }

    }
    public enum ProfileDisplayMode
    {
        Reviews = 1,
        Posts = 2,
        Gallery = 3
    }
}
