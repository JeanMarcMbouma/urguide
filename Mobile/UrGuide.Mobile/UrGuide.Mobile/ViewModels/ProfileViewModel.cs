using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Windows.Input;
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
        private Command _viewReviewsCommand;
        private Command _viewPostsCommand;
        private Command _viewGaleryCommand;

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
        public ObservableRangeCollection<PostItem> Posts { get; set; } = new ObservableRangeCollection<PostItem>
        {
            new PostItem
                {
                    Id = "1",
                    Author = "Jean Marc",
                    AuthorAvatar = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
                    AuthorId = Guid.Empty.ToString(),
                    BidCount = 3,
                    Categories = { "Sport", "Extreme", "Nature"},
                    Text= "Tour around Azov sea",
                    Description = "This is another tour that will help you discover the best out of a million",
                    Dislikes = 1200,
                    IsBidOptIn = true,
                    HasReacted = true,
                    Likes = 1502,
                    ItineraryCount = 4,
                    Price = "$20",
                    Location = "Azov sea, Ukraine",
                    Seats = 10,
                    Images =  {
                        new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/362B092F-5A07-4B03-AA46-BFC181BC6392.png",
                            Name = "Image 1"
                        },
                        new Model.Shared.ImageFileModel
                        {
                            ImageBase64 = "http://urguide.azurewebsites.net/images/A0733818-5052-4642-A650-E154E8539490.png",
                            Name = "Image 2"
                        }
                    },
                    StartDate = "04-Jul-2020",
                    StartTime = "12:09",
                    EndDate = "06-Jul-2020",
                    EndTime = "11:00",
                    PublicationDate = "11-May-2020 11:30:04",
                    Reviews = 2,
                    FeedBack = new ObservableRangeCollection<Model.Shared.AuthoredFeedback>
                    {
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
                    }
                }
        };
        public UrGuide.Model.Shared.FeedbackModel NewFeedBack { get; } = new Model.Shared.FeedbackModel
        {
            Rating = 1
        };
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
