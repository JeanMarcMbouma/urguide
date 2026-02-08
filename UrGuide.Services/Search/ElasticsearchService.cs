using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Search;
using UrGuide.Services.Contracts;

namespace UrGuide.Services.Search
{
    public class ElasticsearchService : IElasticsearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ElasticsearchService> _logger;
        private const string PostsIndexName = "urguide-posts";
        private const string ToursIndexName = "urguide-tours";

        public ElasticsearchService(IElasticClient elasticClient, ILogger<ElasticsearchService> logger)
        {
            _elasticClient = elasticClient ?? throw new ArgumentNullException(nameof(elasticClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Index Management

        public async Task<Result<bool>> CreateIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            try
            {
                var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName, ct: cancellationToken);
                if (existsResponse.Exists)
                {
                    _logger.LogInformation("Index {IndexName} already exists", indexName);
                    return Result.Of(true);
                }

                ICreateIndexRequest GetIndexSettings(string index)
                {
                    if (index == PostsIndexName)
                    {
                        return new CreateIndexRequest(index)
                        {
                            Settings = new IndexSettings
                            {
                                NumberOfShards = 1,
                                NumberOfReplicas = 1
                            },
                            Mappings = new TypeMapping
                            {
                                Properties = new Properties<PostSearchDocument>
                                {
                                    { p => p.Text, new TextProperty { Analyzer = "standard", Fields = new Properties { { "keyword", new KeywordProperty() } } } },
                                    { p => p.Description, new TextProperty { Analyzer = "standard" } },
                                    { p => p.Tags, new KeywordProperty() },
                                    { p => p.GeoLocation, new TextProperty { Analyzer = "standard" } },
                                    { p => p.Location, new GeoPointProperty() },
                                    { p => p.Rating, new NumberProperty(NumberType.Integer) },
                                    { p => p.Cost, new KeywordProperty() },
                                    { p => p.StartDate, new DateProperty() },
                                    { p => p.EndDate, new DateProperty() },
                                    { p => p.DateOfPublication, new DateProperty() }
                                }
                            }
                        };
                    }
                    else if (index == ToursIndexName)
                    {
                        return new CreateIndexRequest(index)
                        {
                            Settings = new IndexSettings
                            {
                                NumberOfShards = 1,
                                NumberOfReplicas = 1
                            },
                            Mappings = new TypeMapping
                            {
                                Properties = new Properties<TourSearchDocument>
                                {
                                    { t => t.Title, new TextProperty { Analyzer = "standard", Fields = new Properties { { "keyword", new KeywordProperty() } } } },
                                    { t => t.Description, new TextProperty { Analyzer = "standard" } },
                                    { t => t.Tags, new KeywordProperty() },
                                    { t => t.AverageRating, new NumberProperty(NumberType.Double) },
                                    { t => t.CreatedAt, new DateProperty() }
                                }
                            }
                        };
                    }

                    return new CreateIndexRequest(index);
                }

                var createResponse = await _elasticClient.Indices.CreateAsync(GetIndexSettings(indexName), ct: cancellationToken);

                if (!createResponse.IsValid)
                {
                    _logger.LogError("Failed to create index {IndexName}: {Error}", indexName, createResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to create index: {createResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully created index {IndexName}", indexName);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating index {IndexName}", indexName);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleteResponse = await _elasticClient.Indices.DeleteAsync(indexName, ct: cancellationToken);
                if (!deleteResponse.IsValid)
                {
                    _logger.LogError("Failed to delete index {IndexName}: {Error}", indexName, deleteResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to delete index: {deleteResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully deleted index {IndexName}", indexName);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting index {IndexName}", indexName);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default)
        {
            try
            {
                var existsResponse = await _elasticClient.Indices.ExistsAsync(indexName, ct: cancellationToken);
                return Result.Of(existsResponse.Exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if index {IndexName} exists", indexName);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        #endregion

        #region Document Indexing

        public async Task<Result<bool>> IndexPostAsync(PostSearchDocument post, CancellationToken cancellationToken = default)
        {
            try
            {
                var indexResponse = await _elasticClient.IndexAsync(post, idx => idx
                    .Index(PostsIndexName)
                    .Id(post.Id)
                    .Refresh(Refresh.WaitFor), cancellationToken);

                if (!indexResponse.IsValid)
                {
                    _logger.LogError("Failed to index post {PostId}: {Error}", post.Id, indexResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to index post: {indexResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully indexed post {PostId}", post.Id);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error indexing post {PostId}", post.Id);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> IndexTourAsync(TourSearchDocument tour, CancellationToken cancellationToken = default)
        {
            try
            {
                var indexResponse = await _elasticClient.IndexAsync(tour, idx => idx
                    .Index(ToursIndexName)
                    .Id(tour.TourId)
                    .Refresh(Refresh.WaitFor), cancellationToken);

                if (!indexResponse.IsValid)
                {
                    _logger.LogError("Failed to index tour {TourId}: {Error}", tour.TourId, indexResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to index tour: {indexResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully indexed tour {TourId}", tour.TourId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error indexing tour {TourId}", tour.TourId);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> BulkIndexPostsAsync(IEnumerable<PostSearchDocument> posts, CancellationToken cancellationToken = default)
        {
            try
            {
                var bulkResponse = await _elasticClient.BulkAsync(b => b
                    .Index(PostsIndexName)
                    .IndexMany(posts)
                    .Refresh(Refresh.WaitFor), cancellationToken);

                if (!bulkResponse.IsValid)
                {
                    _logger.LogError("Failed to bulk index posts: {Error}", bulkResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to bulk index posts: {bulkResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully bulk indexed {Count} posts", posts.Count());
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk indexing posts");
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> BulkIndexToursAsync(IEnumerable<TourSearchDocument> tours, CancellationToken cancellationToken = default)
        {
            try
            {
                var bulkResponse = await _elasticClient.BulkAsync(b => b
                    .Index(ToursIndexName)
                    .IndexMany(tours)
                    .Refresh(Refresh.WaitFor), cancellationToken);

                if (!bulkResponse.IsValid)
                {
                    _logger.LogError("Failed to bulk index tours: {Error}", bulkResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to bulk index tours: {bulkResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully bulk indexed {Count} tours", tours.Count());
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk indexing tours");
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        #endregion

        #region Document Updates

        public async Task<Result<bool>> UpdatePostAsync(PostSearchDocument post, CancellationToken cancellationToken = default)
        {
            try
            {
                var updateResponse = await _elasticClient.UpdateAsync<PostSearchDocument>(post.Id, u => u
                    .Index(PostsIndexName)
                    .Doc(post)
                    .Refresh(Refresh.WaitFor), cancellationToken);

                if (!updateResponse.IsValid)
                {
                    _logger.LogError("Failed to update post {PostId}: {Error}", post.Id, updateResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to update post: {updateResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully updated post {PostId}", post.Id);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId}", post.Id);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateTourAsync(TourSearchDocument tour, CancellationToken cancellationToken = default)
        {
            try
            {
                var updateResponse = await _elasticClient.UpdateAsync<TourSearchDocument>(tour.TourId, u => u
                    .Index(ToursIndexName)
                    .Doc(tour)
                    .Refresh(Refresh.WaitFor), cancellationToken);

                if (!updateResponse.IsValid)
                {
                    _logger.LogError("Failed to update tour {TourId}: {Error}", tour.TourId, updateResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to update tour: {updateResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully updated tour {TourId}", tour.TourId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tour {TourId}", tour.TourId);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        #endregion

        #region Document Deletion

        public async Task<Result<bool>> DeletePostAsync(string postId, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleteResponse = await _elasticClient.DeleteAsync<PostSearchDocument>(postId, d => d
                    .Index(PostsIndexName), cancellationToken);

                if (!deleteResponse.IsValid && deleteResponse.Result != Nest.Result.NotFound)
                {
                    _logger.LogError("Failed to delete post {PostId}: {Error}", postId, deleteResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to delete post: {deleteResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully deleted post {PostId}", postId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId}", postId);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteTourAsync(string tourId, CancellationToken cancellationToken = default)
        {
            try
            {
                var deleteResponse = await _elasticClient.DeleteAsync<TourSearchDocument>(tourId, d => d
                    .Index(ToursIndexName), cancellationToken);

                if (!deleteResponse.IsValid && deleteResponse.Result != Nest.Result.NotFound)
                {
                    _logger.LogError("Failed to delete tour {TourId}: {Error}", tourId, deleteResponse.DebugInformation);
                    return Result.Of(false).WithErrors($"Failed to delete tour: {deleteResponse.ServerError?.Error?.Reason}");
                }

                _logger.LogInformation("Successfully deleted tour {TourId}", tourId);
                return Result.Of(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour {TourId}", tourId);
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        #endregion

        #region Search Operations

        public async Task<Result<SearchResponse<PostSearchDocument>>> SearchPostsAsync(SearchRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var searchDescriptor = BuildPostSearchDescriptor(request);
                var searchResponse = await _elasticClient.SearchAsync<PostSearchDocument>(searchDescriptor, cancellationToken);

                if (!searchResponse.IsValid)
                {
                    _logger.LogError("Failed to search posts: {Error}", searchResponse.DebugInformation);
                    return Result.Of<SearchResponse<PostSearchDocument>>().WithErrors($"Search failed: {searchResponse.ServerError?.Error?.Reason}");
                }

                var response = MapSearchResponse(searchResponse, request);
                return Result.Of(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching posts");
                return Result.Of<SearchResponse<PostSearchDocument>>().WithErrors(ex.Message);
            }
        }

        public async Task<Result<SearchResponse<TourSearchDocument>>> SearchToursAsync(SearchRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var searchDescriptor = BuildTourSearchDescriptor(request);
                var searchResponse = await _elasticClient.SearchAsync<TourSearchDocument>(searchDescriptor, cancellationToken);

                if (!searchResponse.IsValid)
                {
                    _logger.LogError("Failed to search tours: {Error}", searchResponse.DebugInformation);
                    return Result.Of<SearchResponse<TourSearchDocument>>().WithErrors($"Search failed: {searchResponse.ServerError?.Error?.Reason}");
                }

                var response = MapSearchResponse(searchResponse, request);
                return Result.Of(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tours");
                return Result.Of<SearchResponse<TourSearchDocument>>().WithErrors(ex.Message);
            }
        }

        private SearchDescriptor<PostSearchDocument> BuildPostSearchDescriptor(SearchRequest request)
        {
            var descriptor = new SearchDescriptor<PostSearchDocument>()
                .Index(PostsIndexName)
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize)
                .TrackScores();

            // Build query
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                descriptor = descriptor.Query(q => q.Bool(b =>
                {
                    var should = new List<Func<QueryContainerDescriptor<PostSearchDocument>, QueryContainer>>();

                    if (request.FuzzySearch)
                    {
                        should.Add(sq => sq.MultiMatch(m => m
                            .Fields(f => f
                                .Field(p => p.Text, 2.0)
                                .Field(p => p.Description)
                                .Field(p => p.GeoLocation)
                                .Field(p => p.Tags))
                            .Query(request.Query)
                            .Fuzziness(Fuzziness.Auto)
                            .Type(TextQueryType.BestFields)));
                    }
                    else
                    {
                        should.Add(sq => sq.MultiMatch(m => m
                            .Fields(f => f
                                .Field(p => p.Text, 2.0)
                                .Field(p => p.Description)
                                .Field(p => p.GeoLocation)
                                .Field(p => p.Tags))
                            .Query(request.Query)
                            .Type(TextQueryType.BestFields)));
                    }

                    b = b.Should(should.ToArray());
                    b = b.MinimumShouldMatch(1);

                    // Apply filters
                    var filters = BuildPostFilters(request.Filters);
                    if (filters.Any())
                    {
                        b = b.Filter(filters.ToArray());
                    }

                    return b;
                }));
            }
            else
            {
                // No query, just filters
                var filters = BuildPostFilters(request.Filters);
                if (filters.Any())
                {
                    descriptor = descriptor.Query(q => q.Bool(b => b.Filter(filters.ToArray())));
                }
                else
                {
                    descriptor = descriptor.Query(q => q.MatchAll());
                }
            }

            // Add sorting
            descriptor = ApplyPostSorting(descriptor, request);

            // Add aggregations if requested
            if (request.IncludeFacets)
            {
                descriptor = AddPostAggregations(descriptor);
            }

            // Add highlighting
            descriptor = descriptor.Highlight(h => h
                .Fields(
                    f => f.Field(p => p.Text),
                    f => f.Field(p => p.Description),
                    f => f.Field(p => p.GeoLocation)
                )
                .PreTags("<mark>")
                .PostTags("</mark>"));

            return descriptor;
        }

        private SearchDescriptor<TourSearchDocument> BuildTourSearchDescriptor(SearchRequest request)
        {
            var descriptor = new SearchDescriptor<TourSearchDocument>()
                .Index(ToursIndexName)
                .From((request.Page - 1) * request.PageSize)
                .Size(request.PageSize)
                .TrackScores();

            // Build query
            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                descriptor = descriptor.Query(q => q.Bool(b =>
                {
                    var should = new List<Func<QueryContainerDescriptor<TourSearchDocument>, QueryContainer>>();

                    if (request.FuzzySearch)
                    {
                        should.Add(sq => sq.MultiMatch(m => m
                            .Fields(f => f
                                .Field(t => t.Title, 2.0)
                                .Field(t => t.Description)
                                .Field(t => t.Tags))
                            .Query(request.Query)
                            .Fuzziness(Fuzziness.Auto)
                            .Type(TextQueryType.BestFields)));
                    }
                    else
                    {
                        should.Add(sq => sq.MultiMatch(m => m
                            .Fields(f => f
                                .Field(t => t.Title, 2.0)
                                .Field(t => t.Description)
                                .Field(t => t.Tags))
                            .Query(request.Query)
                            .Type(TextQueryType.BestFields)));
                    }

                    b = b.Should(should.ToArray());
                    b = b.MinimumShouldMatch(1);

                    // Apply filters
                    var filters = BuildTourFilters(request.Filters);
                    if (filters.Any())
                    {
                        b = b.Filter(filters.ToArray());
                    }

                    return b;
                }));
            }
            else
            {
                // No query, just filters
                var filters = BuildTourFilters(request.Filters);
                if (filters.Any())
                {
                    descriptor = descriptor.Query(q => q.Bool(b => b.Filter(filters.ToArray())));
                }
                else
                {
                    descriptor = descriptor.Query(q => q.MatchAll());
                }
            }

            // Add sorting
            descriptor = ApplyTourSorting(descriptor, request);

            // Add aggregations if requested
            if (request.IncludeFacets)
            {
                descriptor = AddTourAggregations(descriptor);
            }

            // Add highlighting
            descriptor = descriptor.Highlight(h => h
                .Fields(
                    f => f.Field(t => t.Title),
                    f => f.Field(t => t.Description)
                )
                .PreTags("<mark>")
                .PostTags("</mark>"));

            return descriptor;
        }

        private List<Func<QueryContainerDescriptor<PostSearchDocument>, QueryContainer>> BuildPostFilters(SearchFilters filters)
        {
            var result = new List<Func<QueryContainerDescriptor<PostSearchDocument>, QueryContainer>>();

            // Location filter
            if (!string.IsNullOrWhiteSpace(filters.Location))
            {
                result.Add(f => f.Match(m => m.Field(p => p.GeoLocation).Query(filters.Location)));
            }

            // Geo distance filter
            if (filters.Latitude.HasValue && filters.Longitude.HasValue && !string.IsNullOrWhiteSpace(filters.Distance))
            {
                result.Add(f => f.GeoDistance(g => g
                    .Field(p => p.Location)
                    .Location(filters.Latitude.Value, filters.Longitude.Value)
                    .Distance(filters.Distance)));
            }

            // Price range filter
            if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
            {
                result.Add(f => f.Range(r =>
                {
                    var range = r.Field(p => p.Cost);
                    if (filters.MinPrice.HasValue)
                        range = range.GreaterThanOrEquals(filters.MinPrice.Value.ToString());
                    if (filters.MaxPrice.HasValue)
                        range = range.LessThanOrEquals(filters.MaxPrice.Value.ToString());
                    return range;
                }));
            }

            // Rating filter
            if (filters.MinRating.HasValue)
            {
                result.Add(f => f.Range(r => r.Field(p => p.Rating).GreaterThanOrEquals(filters.MinRating.Value)));
            }
            if (filters.MaxRating.HasValue)
            {
                result.Add(f => f.Range(r => r.Field(p => p.Rating).LessThanOrEquals(filters.MaxRating.Value)));
            }

            // Date filters
            if (filters.StartDateFrom.HasValue)
            {
                result.Add(f => f.DateRange(r => r.Field(p => p.StartDate).GreaterThanOrEquals(filters.StartDateFrom.Value)));
            }
            if (filters.StartDateTo.HasValue)
            {
                result.Add(f => f.DateRange(r => r.Field(p => p.StartDate).LessThanOrEquals(filters.StartDateTo.Value)));
            }
            if (filters.EndDateFrom.HasValue)
            {
                result.Add(f => f.DateRange(r => r.Field(p => p.EndDate).GreaterThanOrEquals(filters.EndDateFrom.Value)));
            }
            if (filters.EndDateTo.HasValue)
            {
                result.Add(f => f.DateRange(r => r.Field(p => p.EndDate).LessThanOrEquals(filters.EndDateTo.Value)));
            }

            // Tags filter
            if (filters.Tags != null && filters.Tags.Any())
            {
                result.Add(f => f.Terms(t => t.Field(p => p.Tags).Terms(filters.Tags)));
            }

            // Available seats filter
            if (filters.AvailableSeatsOnly.HasValue && filters.AvailableSeatsOnly.Value)
            {
                result.Add(f => f.Range(r => r.Field(p => p.AvailableSeats).GreaterThan(0)));
            }
            if (filters.MinSeats.HasValue)
            {
                result.Add(f => f.Range(r => r.Field(p => p.AvailableSeats).GreaterThanOrEquals(filters.MinSeats.Value)));
            }

            // Bid enabled filter
            if (filters.BidEnabled.HasValue)
            {
                result.Add(f => f.Term(t => t.Field(p => p.BidEnabled).Value(filters.BidEnabled.Value)));
            }

            // User filter
            if (!string.IsNullOrWhiteSpace(filters.UserId))
            {
                result.Add(f => f.Term(t => t.Field(p => p.UserId).Value(filters.UserId)));
            }

            return result;
        }

        private List<Func<QueryContainerDescriptor<TourSearchDocument>, QueryContainer>> BuildTourFilters(SearchFilters filters)
        {
            var result = new List<Func<QueryContainerDescriptor<TourSearchDocument>, QueryContainer>>();

            // Rating filter
            if (filters.MinRating.HasValue)
            {
                result.Add(f => f.Range(r => r.Field(t => t.AverageRating).GreaterThanOrEquals(filters.MinRating.Value)));
            }
            if (filters.MaxRating.HasValue)
            {
                result.Add(f => f.Range(r => r.Field(t => t.AverageRating).LessThanOrEquals(filters.MaxRating.Value)));
            }

            // Tags filter
            if (filters.Tags != null && filters.Tags.Any())
            {
                result.Add(f => f.Terms(t => t.Field(p => p.Tags).Terms(filters.Tags)));
            }

            // Author filter
            if (!string.IsNullOrWhiteSpace(filters.UserId))
            {
                result.Add(f => f.Term(t => t.Field(p => p.AuthorId).Value(filters.UserId)));
            }

            return result;
        }

        private SearchDescriptor<PostSearchDocument> ApplyPostSorting(SearchDescriptor<PostSearchDocument> descriptor, SearchRequest request)
        {
            var order = request.SortOrder?.ToLower() == "asc" ? SortOrder.Ascending : SortOrder.Descending;

            return request.SortBy?.ToLower() switch
            {
                "date" => descriptor.Sort(s => s.Field(f => f.Field(p => p.DateOfPublication).Order(order))),
                "rating" => descriptor.Sort(s => s.Field(f => f.Field(p => p.Rating).Order(order))),
                "price" => descriptor.Sort(s => s.Field(f => f.Field(p => p.Cost).Order(order))),
                _ => descriptor.Sort(s => s.Score().Descending())
            };
        }

        private SearchDescriptor<TourSearchDocument> ApplyTourSorting(SearchDescriptor<TourSearchDocument> descriptor, SearchRequest request)
        {
            var order = request.SortOrder?.ToLower() == "asc" ? SortOrder.Ascending : SortOrder.Descending;

            return request.SortBy?.ToLower() switch
            {
                "date" => descriptor.Sort(s => s.Field(f => f.Field(t => t.CreatedAt).Order(order))),
                "rating" => descriptor.Sort(s => s.Field(f => f.Field(t => t.AverageRating).Order(order))),
                _ => descriptor.Sort(s => s.Score().Descending())
            };
        }

        private SearchDescriptor<PostSearchDocument> AddPostAggregations(SearchDescriptor<PostSearchDocument> descriptor)
        {
            return descriptor.Aggregations(a => a
                .Terms("tags", t => t.Field(p => p.Tags).Size(20))
                .Terms("locations", t => t.Field(p => p.GeoLocation.Suffix("keyword")).Size(20))
                .Terms("ratings", t => t.Field(p => p.Rating))
                .Range("price_ranges", r => r
                    .Field(p => p.Cost)
                    .Ranges(
                        ranges => ranges.To("50"),
                        ranges => ranges.From("50").To("100"),
                        ranges => ranges.From("100").To("200"),
                        ranges => ranges.From("200")))
            );
        }

        private SearchDescriptor<TourSearchDocument> AddTourAggregations(SearchDescriptor<TourSearchDocument> descriptor)
        {
            return descriptor.Aggregations(a => a
                .Terms("tags", t => t.Field(p => p.Tags).Size(20))
                .Terms("regions", t => t.Field(p => p.RegionName.Suffix("keyword")).Size(20))
                .Histogram("ratings", h => h.Field(t => t.AverageRating).Interval(1).MinimumDocumentCount(0))
            );
        }

        private SearchResponse<T> MapSearchResponse<T>(ISearchResponse<T> elasticResponse, SearchRequest request) where T : class
        {
            var response = new SearchResponse<T>
            {
                TotalHits = elasticResponse.Total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)elasticResponse.Total / request.PageSize),
                TimeTakenMs = elasticResponse.Took,
                Results = elasticResponse.Hits.Select(h => new SearchResultItem<T>
                {
                    Document = h.Source,
                    Score = h.Score ?? 0,
                    Highlights = h.Highlight?.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.ToList())
                }).ToList()
            };

            // Map aggregations if present
            if (request.IncludeFacets && elasticResponse.Aggregations != null)
            {
                response.Facets = new SearchFacets();

                // Tags facet
                if (elasticResponse.Aggregations.Terms("tags") != null)
                {
                    response.Facets.TagsFacet = elasticResponse.Aggregations.Terms("tags").Buckets
                        .ToDictionary(b => b.Key, b => b.DocCount ?? 0);
                }

                // Locations facet
                if (elasticResponse.Aggregations.Terms("locations") != null)
                {
                    response.Facets.LocationsFacet = elasticResponse.Aggregations.Terms("locations").Buckets
                        .ToDictionary(b => b.Key, b => b.DocCount ?? 0);
                }

                // Regions facet
                if (elasticResponse.Aggregations.Terms("regions") != null)
                {
                    response.Facets.LocationsFacet = elasticResponse.Aggregations.Terms("regions").Buckets
                        .ToDictionary(b => b.Key, b => b.DocCount ?? 0);
                }

                // Rating facet
                if (elasticResponse.Aggregations.Terms("ratings") != null)
                {
                    response.Facets.RatingDistribution = elasticResponse.Aggregations.Terms("ratings").Buckets
                        .Select(b => new RatingFacet
                        {
                            Rating = int.TryParse(b.Key, out var rating) ? rating : 0,
                            Count = b.DocCount ?? 0
                        })
                        .ToList();
                }

                // Histogram rating facet for tours
                if (elasticResponse.Aggregations.Histogram("ratings") != null)
                {
                    response.Facets.RatingDistribution = elasticResponse.Aggregations.Histogram("ratings").Buckets
                        .Select(b => new RatingFacet
                        {
                            Rating = (int)b.Key,
                            Count = b.DocCount ?? 0
                        })
                        .ToList();
                }

                // Price ranges facet
                if (elasticResponse.Aggregations.Range("price_ranges") != null)
                {
                    response.Facets.PriceRanges = elasticResponse.Aggregations.Range("price_ranges").Buckets
                        .Select(b => new PriceRangeFacet
                        {
                            Range = b.Key,
                            Count = b.DocCount ?? 0,
                            From = b.From.HasValue ? (decimal?)b.From : null,
                            To = b.To.HasValue ? (decimal?)b.To : null
                        })
                        .ToList();
                }
            }

            return response;
        }

        #endregion

        #region Autocomplete

        public async Task<Result<AutocompleteResponse>> AutocompleteAsync(AutocompleteRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = new AutocompleteResponse();

                if (request.Type == "all" || request.Type == "posts")
                {
                    var postSuggestions = await GetPostSuggestions(request.Query, request.Size, cancellationToken);
                    response.Suggestions.AddRange(postSuggestions);
                }

                if (request.Type == "all" || request.Type == "tours")
                {
                    var tourSuggestions = await GetTourSuggestions(request.Query, request.Size, cancellationToken);
                    response.Suggestions.AddRange(tourSuggestions);
                }

                return Result.Of(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting autocomplete suggestions");
                return Result.Of<AutocompleteResponse>().WithErrors(ex.Message);
            }
        }

        private async Task<List<AutocompleteSuggestion>> GetPostSuggestions(string query, int size, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<PostSearchDocument>(s => s
                .Index(PostsIndexName)
                .Size(size)
                .Query(q => q.Match(m => m.Field(p => p.Text).Query(query).Fuzziness(Fuzziness.Auto)))
                .Source(sf => sf.Includes(i => i.Fields(f => f.Text, f => f.Id))),
                cancellationToken);

            if (!searchResponse.IsValid)
                return new List<AutocompleteSuggestion>();

            return searchResponse.Documents.Select(d => new AutocompleteSuggestion
            {
                Text = d.Text,
                Type = "post",
                Score = searchResponse.Hits.FirstOrDefault(h => h.Source.Id == d.Id)?.Score ?? 0
            }).ToList();
        }

        private async Task<List<AutocompleteSuggestion>> GetTourSuggestions(string query, int size, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<TourSearchDocument>(s => s
                .Index(ToursIndexName)
                .Size(size)
                .Query(q => q.Match(m => m.Field(t => t.Title).Query(query).Fuzziness(Fuzziness.Auto)))
                .Source(sf => sf.Includes(i => i.Fields(f => f.Title, f => f.TourId))),
                cancellationToken);

            if (!searchResponse.IsValid)
                return new List<AutocompleteSuggestion>();

            return searchResponse.Documents.Select(d => new AutocompleteSuggestion
            {
                Text = d.Title,
                Type = "tour",
                Score = searchResponse.Hits.FirstOrDefault(h => h.Source.TourId == d.TourId)?.Score ?? 0
            }).ToList();
        }

        #endregion

        #region Utility

        public async Task<Result<bool>> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var pingResponse = await _elasticClient.PingAsync(ct: cancellationToken);
                return Result.Of(pingResponse.IsValid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch health check failed");
                return Result.Of(false).WithErrors(ex.Message);
            }
        }

        #endregion
    }
}
