using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Catalogs;
using UrGuide.Model.Posts;
using UrGuide.Model.Shared;
using UrGuide.Model.Users;

namespace UrGuide.Mobile.ViewModels
{
    class ProfileViewModel : BaseViewModel
    {
        private ProfileDisplayMode mode = ProfileDisplayMode.Reviews;
        private ICommand _viewReviewsCommand;
        private ICommand _viewPostsCommand;
        private ICommand _viewGaleryCommand;
        private ICommand _viewPostDetailsCommand;
        private ICommand _loadItemsCommand;
        public ProfileViewModel(INavigationService navigation, IPostItemService postItemService)
        {
            Navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
            PostItemService = postItemService ?? throw new ArgumentNullException(nameof(postItemService));
        }

        public UserInfo UserInfo { get; set; } = new UserInfo
        {
            City = "Yaounde",
            Country = "Cameroon",
            Description = "I am the guide you were looking for.",
            Rating = 4,
            FirstName = "Jean Marc",
            LastName = "Mbouma",
            FullName = "Jean Marc Mbouma",
            ProfileImage = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
        };

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
        public ICommand LoadItemsCommand => _loadItemsCommand ??= new Command(async () =>
        {
            IsBusy = true;
            var items = await PostItemService.GetItemsAsync();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Posts.ReplaceRange(items);
            });
            IsBusy = false;
        });
        public ICommand ViewPostDetailsCommand => _viewPostDetailsCommand ??= new Command<PostItem>(async (item) => await Navigation.GotoAsync($"postdetails?Id={item.Id}"));
        public ICommand ViewReviewsCommand => _viewReviewsCommand ??= new Command(() => Mode = ProfileDisplayMode.Reviews);
        public ICommand ViewPostsCommand => _viewPostsCommand ??= new Command(() => Mode = ProfileDisplayMode.Posts);
        public ICommand ViewGalleryCommand => _viewGaleryCommand ??= new Command(() => Mode = ProfileDisplayMode.Gallery);
        public ObservableRangeCollection<AuthoredFeedback> Feedbacks { get; set; } = new ObservableRangeCollection<AuthoredFeedback> {
        new Model.Shared.AuthoredFeedback
                        {
                            Rating = 4,
                            Text = "I love this guy",
                            AuthorFullName = "Catherine Dubois",
                            AuthorId = Guid.Empty.ToString(),
                            AuthorImage = "http://urguide.azurewebsites.net/thumb/00000000-0000-0000-0000-000000000000.png",
                            PublicationDate = "12-Jun-2020 12:45:02"
                        },
                        new Model.Shared.AuthoredFeedback
                        {
                            Rating = 5,
                            Text = "Lucky you!",
                            AuthorFullName = "Alain Dubois",
                            AuthorId = Guid.Empty.ToString(),
                            AuthorImage = "http://urguide.azurewebsites.net/thumb/00000000-0000-0000-0000-000000000000.png",
                            PublicationDate = "12-Jun-2020 12:45:02"
                        }
        };
        public ObservableRangeCollection<GalleryItem> Catalogs { get; set; } = new ObservableRangeCollection<GalleryItem>
        {
            new GalleryItem
            {
                Name = "Yaounde",
                Description = "The city of 7 hills",
                Files = new ObservableRangeCollection<ImageFileModel>
                {
                    new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/362B092F-5A07-4B03-AA46-BFC181BC6392.png",
                            Name = "Image 1"
                        }
                        ,new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/A0733818-5052-4642-A650-E154E8539490.png",
                            Name = "Image 1"
                        }
                }
            },
            new GalleryItem
            {
                Name = "Douala",
                Description = "Deido beach",
                Files = new ObservableRangeCollection<ImageFileModel>
                {
                    new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/362B092F-5A07-4B03-AA46-BFC181BC6392.png",
                            Name = "Image 1"
                        }
                        ,new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/A0733818-5052-4642-A650-E154E8539490.png",
                            Name = "Image 1"
                        }
                }
            },
            new GalleryItem
            {
                Name = "Edea",
                Description = "The sanaga maritime",
                Files = new ObservableRangeCollection<ImageFileModel>
                {
                    new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/362B092F-5A07-4B03-AA46-BFC181BC6392.png",
                            Name = "Image 1"
                        }
                        ,new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/A0733818-5052-4642-A650-E154E8539490.png",
                            Name = "Image 1"
                        }
                }
            }
        };
        public ObservableRangeCollection<PostItem> Posts { get; set; } = new ObservableRangeCollection<PostItem>();
        public UrGuide.Model.Shared.FeedbackModel NewFeedBack { get; } = new Model.Shared.FeedbackModel
        {
            Rating = 1
        };
        public INavigationService Navigation { get; }
        public IPostItemService PostItemService { get; }

        public class WrapperViewModel : BaseViewModel
        {
            private ProfileDisplayMode mode = ProfileDisplayMode.Reviews;

            public ProfileDisplayMode Mode { get => mode; set => SetProperty(ref mode, value); }

        }

    }
    public enum ProfileDisplayMode
    {
        Reviews = 1,
        Posts = 2,
        Gallery = 3
    }
}
