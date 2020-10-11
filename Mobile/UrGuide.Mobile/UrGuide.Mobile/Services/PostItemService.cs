using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Mobile.API;
using UrGuide.Mobile.Contracts;
using UrGuide.Mobile.Models;
using UrGuide.Model.Results;
using System.Reactive.Linq;
using Akavache;
using System.Net.Http;
using Sharpnado.Presentation.Forms.Services;
using System.Collections.ObjectModel;

namespace UrGuide.Mobile.Services
{
    class PostItemService : IPostItemService
    {
        private const string Favorites_CacheKey = "favorites";

        public PostItemService(IHttpClientFactory clientFactory,
                               BidClient bidClient,
                               FeedbackClient feedbackClient,
                               LookupClient lookupClient,
                               IMapper mapper,
                               IBlobCache cache,
                               IPreferenceService preference)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            BidClient = bidClient ?? throw new ArgumentNullException(nameof(bidClient));
            FeedbackClient = feedbackClient ?? throw new ArgumentNullException(nameof(feedbackClient));
            LookupClient = lookupClient ?? throw new ArgumentNullException(nameof(lookupClient));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            Preference = preference ?? throw new ArgumentNullException(nameof(preference));
        }

        IHttpClientFactory _clientFactory;
        public PostsClient Client
        {
            get
            {
                return new PostsClient(_clientFactory.CreateClient(nameof(PostsClient)));
            }
        }

        public BidClient BidClient { get; }
        public FeedbackClient FeedbackClient { get; }
        public LookupClient LookupClient { get; }
        public IMapper Mapper { get; }
        public IBlobCache Cache { get; }
        public IPreferenceService Preference { get; }

        public async Task<Result<IEnumerable<Model.Posts.BidHistoryModel>>> GetBidHistoryAsync(string id)
        {
            return await Cache.GetOrFetchObject($"bid_{id}", async () =>
            {
                var bidHistory = await BidClient.HistoryAsync(id);
                return Result.Of(Mapper.Map<IEnumerable<Model.Posts.BidHistoryModel>>(bidHistory));
            }).Catch(Observable.Return(Result.Of<IEnumerable<Model.Posts.BidHistoryModel>>().WithErrors("Error occured")));
        }

        public async Task<Result<PostItem>> GetByIdAsync(string id)
        {
            var item = await Cache.GetOrFetchObject($"post_{id}", async () =>
            {
                var post = await Client.RetrieveAsync(id);
                var model = Mapper.Map<PostItem>(post);
                return Result.Of(model);
            }).Catch(Observable.Return(Result.Of<PostItem>().WithErrors("Error occured")));
            var favorites = await GetFavoriteAsync();
            if (!item.HasError)
                item.Data.Favorite = favorites.Any(f => f.Id == item.Data.Id);
            return item;
        }

        public async Task<Result<IEnumerable<Model.Lookup.CategoryModel>>> GetCategoriesAsync()
        {
            return await Cache.GetOrFetchObject("categories", async () =>
            {
                var categories = await LookupClient.CategoriesAsync();
                return Result.Of(Mapper.Map<IEnumerable<Model.Lookup.CategoryModel>>(categories));
            }, DateTime.UtcNow.AddYears(1)).Catch(Observable.Return(Result.Of<IEnumerable<Model.Lookup.CategoryModel>>().WithErrors("Error occured")));
        }

        public async Task Create(PostCreationModel post)
        {
            await Client.CreateAsync(post);
        }

        public async Task<IEnumerable<PostItem>> GetFavoriteAsync()
        {
            return await Cache.GetOrCreateObject(Favorites_CacheKey, () => new List<PostItem>());
        }

        public async Task<Result<IEnumerable<PostItem>>> GetItemsAsync()
        {
            try
            {
                var posts = await Client.Last10Async();
                var favorites = await GetFavoriteAsync();
                var items = Mapper.Map<IEnumerable<PostItem>>(posts).Select(x =>
                    {
                        x.Favorite = favorites.Any(f => f.Id == x.Id);
                        return x;
                    });
                return Result.Of(items);
            }
            catch (ApiException e)
            {

                return Result.Of<IEnumerable<PostItem>>().WithErrors(e.Message);
            }
            
        }

        public async Task<PageResult<Model.Shared.AuthoredFeedback>> GetPostFeedbackAsync(string id, int pageNumber = 1)
        {
            var feedbackPagedList = await FeedbackClient.PostsAsync(id, pageNumber);
            return new PageResult<Model.Shared.AuthoredFeedback>(feedbackPagedList.ItemsCount, Mapper.Map<IEnumerable<Model.Shared.AuthoredFeedback>>(feedbackPagedList.Items).ToList());
        }

        public async Task<Result<IEnumerable<DiscoverItem>>> SearchAsync(bool nearby, IEnumerable<string> categories = null, string searchTerm = null, int pageNumber = 1)
        {
            try
            {
                var posts = await Client.SearchAsync(new SearchParameters
                {
                    Extra = categories.ToList(),
                    Nearby = nearby,
                    PageNumber = pageNumber,
                    Term = searchTerm
                }).ConfigureAwait(false);

                return Result.Of(Mapper.Map<IEnumerable<DiscoverItem>>(posts.Items.AsEnumerable()));
            }
            catch (ApiException e)
            {

                return Result.Of<IEnumerable<DiscoverItem>>().WithErrors(e.Message);
            }
        }

        public async Task SetUserReaction(PostItem it)
        {
            if (it.HasReacted)
            {
                it.Likes--;
                it.HasReacted = false;
                it.ReactionType = GlobalSetting.Unknown;
            } 
            else
            {
                it.Likes++;
                it.HasReacted = true;
                it.ReactionType = GlobalSetting.Like;
            }
            try
            {
                await Client.ReactionAsync(it.Id, new UserReactionModel
                {
                    Like = true,
                    PostId = it.Id
                });
            }
            catch (ApiException e)
            {
                // do something as the user might have lost his authorization
            }
        }

        public async Task ToggleFavorites(PostItem it)
        {
            var favorites = await Cache.GetOrCreateObject(Favorites_CacheKey, () => new List<PostItem>());
            var item = favorites.FirstOrDefault(f => f.Id.Equals(it.Id));
            if (item != null)
            {
                item.Favorite = false;
                favorites.Remove(item);
            } else
            {
                it.Favorite = true;
                favorites.Add(it);
            }
            await Cache.Invalidate(Favorites_CacheKey);
            await Cache.InsertObject(Favorites_CacheKey, favorites, TimeSpan.FromDays(360));
        }

        public async Task<Result<Model.Shared.AuthoredFeedback>> SendFeedback(string postId, Model.Shared.FeedbackModel item)
        {
            try
            {
                var result = await Client.FeedbackAsync(postId, new FeedbackModel
                {
                    Rating = item.Rating,
                    Text = item.Text
                });
                if (result)
                {
                    return Result.Of(new Model.Shared.AuthoredFeedback
                    {
                        AuthorFullName = Preference.FullName,
                        AuthorId = Preference.UserId,
                        AuthorImage = Preference.Image,
                        Rating = item.Rating,
                        Text = item.Text,
                        PublicationDate = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss")
                    });
                }
                return Result.Of<Model.Shared.AuthoredFeedback>().WithErrors("Fail to post your feedback, please try later");
            }
            catch (ApiException e)
            {
                return Result.Of<Model.Shared.AuthoredFeedback>().WithErrors(e.Message);
            }
        }
    }
}
