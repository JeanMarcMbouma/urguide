using AutoMapper;
using MvvmHelpers;
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

namespace UrGuide.Mobile.Services
{
    class PostItemService : IPostItemService
    {
        private const string Favorites_CacheKey = "favorites";

        public PostItemService(PostsClient client,
                               BidClient bidClient,
                               FeedbackClient feedbackClient,
                               LookupClient lookupClient,
                               IMapper mapper,
                               IBlobCache cache)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            BidClient = bidClient ?? throw new ArgumentNullException(nameof(bidClient));
            FeedbackClient = feedbackClient ?? throw new ArgumentNullException(nameof(feedbackClient));
            LookupClient = lookupClient ?? throw new ArgumentNullException(nameof(lookupClient));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public PostsClient Client { get; }
        public BidClient BidClient { get; }
        public FeedbackClient FeedbackClient { get; }
        public LookupClient LookupClient { get; }
        public IMapper Mapper { get; }
        public IBlobCache Cache { get; }

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
                var feedback = await GetPostFeedbackAsync(id);
                if (!feedback.HasError)
                    model.FeedBack.ReplaceRange(feedback.Data);
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
            }).Catch(Observable.Return(Result.Of<IEnumerable<Model.Lookup.CategoryModel>>().WithErrors("Error occured")));
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

        public async Task<Result<IEnumerable<Model.Shared.AuthoredFeedback>>> GetPostFeedbackAsync(string id, int pageNumber = 1)
        {
            try
            {
                var feedbackPagedList = await FeedbackClient.PostsAsync(id, pageNumber);
                return Result.Of(Mapper.Map<IEnumerable<Model.Shared.AuthoredFeedback>>(feedbackPagedList.Items));
            }
            catch (ApiException e)
            {
                return Result.Of<IEnumerable<Model.Shared.AuthoredFeedback>>().WithErrors(e.Message);
            }
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
            await Cache.InsertObject(Favorites_CacheKey, favorites);
        }
    }
}
