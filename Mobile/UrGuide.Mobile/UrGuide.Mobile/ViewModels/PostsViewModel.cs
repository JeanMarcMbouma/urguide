using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Posts;

namespace UrGuide.Mobile.ViewModels
{
    public class PostsViewModel : BaseViewModel
    {
        public const int Like = 2;
        public const int DisLike = 4;
        public const int Unknown = 0;
        private PostItem selected;
        private readonly INavigationService _navigation;
        private ICommand _viewDetailCommand;
        private ICommand _likeCommand;
        private ICommand _dislikeCommand;
        public ICommand ViewDetailsCommand => _viewDetailCommand ??=
            new AsyncCommand<PostItem>(async (item) =>
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Selected = item;
                });
                
                var it = Items.First(x => x.Id == item.Id);
                await _navigation.PushAsync(new Views.PostDetailPage(new PostDetailViewModel(it)), true);
            });

        public ICommand LikeCommand => _likeCommand ??= new Command<PostItem>((item) =>
        {
            var it = Items.First(x => x.Id == item.Id);
            if (it.ReactionType == Like)
            {
                it.Likes--;
                it.ReactionType = Unknown;
                return;
            }
            if (it.ReactionType == DisLike)
            {
                it.Dislikes--;
            }
            it.Likes++;
            it.ReactionType = Like;
        });

        public ICommand DislikeCommand => _dislikeCommand ??= new Command<PostItem>((item) =>
        {
            var it = Items.First(x => x.Id == item.Id);
            if (it.ReactionType == DisLike)
            { 
                it.Dislikes--;
                it.ReactionType = Unknown;
                return;
            }
            if (it.ReactionType == Like)
            { 
                it.Likes--; 
            }
            it.Dislikes++;
            it.ReactionType = DisLike;
        });

        public ObservableRangeCollection<PostItem> Items { get; }
        public PostItem Selected
        {
            get => selected; 
            set
            {
                SetProperty(ref selected, value);
            }
        }
        public PostsViewModel(INavigationService navigation)
        {
            Items = new ObservableRangeCollection<PostItem>
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
                    Itineraries = new List<ItineraryModel>
                    {
                        new ItineraryModel
                        {
                            Title = "Yaounde",
                            Description = "The capital of Cameroon"
                        },
                        new ItineraryModel
                        {
                            Title = "Douala",
                            Description = "The economic capital of Cameroon"
                        }
                    },
                    ReactionType = Like
                },
                 new PostItem
                {
                     Id = "2",
                    Author = "Ivan Markovich",
                    AuthorAvatar = "http://urguide.azurewebsites.net/thumb/00000000-0000-0000-0000-000000000000.png",
                    AuthorId = Guid.Empty.ToString(),
                    BidCount = 3,
                    Categories = { "Extreme", "Kids"},
                    Text = "Tour around Dniepr",
                    Description = "Like  it or not, you'll discover the wonderful world made by Jehovah",
                    Dislikes = 10,
                    IsBidOptIn = true,
                    HasReacted = true,
                    Likes = 1502000,
                    ItineraryCount = 4,
                    Price = "$20",
                    Location = "Cherkassy, Ukraine",
                    Seats = 10,
                    Images = {
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
                    },
                    StartDate = "12-Jul-2020",
                    StartTime = "12:09",
                    EndDate = "28-Jul-2020",
                    EndTime = "11:00",
                    PublicationDate = "11-May-2020 11:30:04",
                    Itineraries = new List<ItineraryModel>
                    {
                        new ItineraryModel
                        {
                            Title = "Yaounde",
                            Description = "The capital of Cameroon"
                        },
                        new ItineraryModel
                        {
                            Title = "Douala",
                            Description = "The economic capital of Cameroon"
                        }
                    }
                }
            };
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }
    }
}
