using AutoMapper;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Mobile.API;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Mobile.Views;
using UrGuide.Model.Posts;
using UrGuide.Model.Results;
using Xamarin.Forms;

namespace UrGuide.Mobile.Services
{
    class PostItemService : IPostItemService
    {
        public PostItemService(PostsClient client, IMapper mapper)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public PostsClient Client { get; }
        public IMapper Mapper { get; }

        public PostItem GetById(string id)
        {
            var post = Client.RetrieveAsync(id).GetAwaiter().GetResult();
            return Mapper.Map<PostItem>(post);
        }

        public Task<IEnumerable<PostItem>> GetFavoriteAsync()
        {
            return Task.FromResult(new []{
                new PostItem
                {
                    Id = "1",
                    Author = "Jean Marc",
                    AuthorAvatar = "http://urguide.azurewebsites.net/images/85e526dd-6b92-4700-b427-6c7d7fe40a45.png",
                    AuthorId = Guid.Empty.ToString(),
                    BidCount = 3,
                    Categories = { "Sport", "Extreme", "Nature" },
                    Text = "Tour around Azov sea",
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
                    Itineraries = new List<Model.Posts.ItineraryModel>
                    {
                        new Model.Posts.ItineraryModel
                        {
                            Title = "Yaounde",
                            Description = "The capital of Cameroon"
                        },
                        new Model.Posts.ItineraryModel
                        {
                            Title = "Douala",
                            Description = "The economic capital of Cameroon"
                        }
                    },
                    ReactionType = 1,
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
                    },
                    Favorite= true
                }
            }.AsEnumerable());
        }

        public async Task<IEnumerable<PostItem>> GetItemsAsync()
        {
            var posts = await Client.Last10Async();
            return Mapper.Map<IEnumerable<PostItem>>(posts);
        }

        public Result<IEnumerable<DiscoverItem>> Search(bool nearby, IEnumerable<string> categories = null, string searchTerm = null)
        {
            var posts = Client.SearchAsync(new SearchParameters
            {
                Extra = categories.ToList(),
                Nearby = nearby,
                PageNumber = 1,
                Term = searchTerm
            }).GetAwaiter().GetResult();
            return Result.Of(Mapper.Map<IEnumerable<DiscoverItem>>(posts.Items.AsEnumerable()));
        }
    }
}
