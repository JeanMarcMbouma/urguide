using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using UrGuide.Mobile.Models;
using UrGuide.Model.Posts;
using Xamarin.Essentials;

namespace UrGuide.Mobile.ViewModels
{
    class MainPageViewModel : BaseViewModel
    {
        public ObservableRangeCollection<PostItem> Items { get; }

        public MainPageViewModel()
        {
            Items = new ObservableRangeCollection<PostItem>
            {
                new PostItem
                {
                    Author = "Jean Marc",
                    AuthorAvatar = "http://urguide.azurewebsites.net/thumb/00000000-0000-0000-0000-000000000000.png",
                    AuthorId = Guid.Empty.ToString(),
                    BidCount = 3,
                    Categories = { "Sport", "Extreme", "Nature"},
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
                        }
                    },
                    StartDate = "12-May-2020",
                    StartTime = "12:09",
                    EndDate = "28-May-2020",
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
                },
                 new PostItem
                {
                    Author = "Ivan Markovich",
                    AuthorAvatar = "http://urguide.azurewebsites.net/thumb/00000000-0000-0000-0000-000000000000.png",
                    AuthorId = Guid.Empty.ToString(),
                    BidCount = 3,
                    Categories = { "Extreme", "Kids"},
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
                    StartDate = "12-May-2020",
                    StartTime = "12:09",
                    EndDate = "28-May-2020",
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
        }
    }
}
