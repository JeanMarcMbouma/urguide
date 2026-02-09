using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Data;
using UrGuide.Model.Search;
using UrGuide.Services.Contracts;
using UrGuide.Services.Search;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IElasticsearchService _elasticsearchService;
        private readonly ISearchAnalyticsService _searchAnalyticsService;
        private readonly IUserContext _userContext;
        private readonly ILogger<SearchController> _logger;
        private readonly UrGuideContext _context;

        public SearchController(
            IElasticsearchService elasticsearchService,
            ISearchAnalyticsService searchAnalyticsService,
            IUserContext userContext,
            ILogger<SearchController> logger,
            UrGuideContext context)
        {
            _elasticsearchService = elasticsearchService ?? throw new ArgumentNullException(nameof(elasticsearchService));
            _searchAnalyticsService = searchAnalyticsService ?? throw new ArgumentNullException(nameof(searchAnalyticsService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Search for posts using advanced Elasticsearch capabilities
        /// </summary>
        /// <param name="request">Search request with query, filters, and pagination</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Search results with facets and highlights</returns>
        [HttpPost("posts")]
        [ProducesResponseType(typeof(SearchResponse<PostSearchDocument>), 200)]
        [ProducesResponseType(typeof(ErrorEnvelop<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchPosts([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ErrorEnvelop.Create(new[] { "Search request is required" }));
                }

                var startTime = DateTime.UtcNow;
                var result = await _elasticsearchService.SearchPostsAsync(request, cancellationToken);

                if (result.HasError)
                {
                    return BadRequest(ErrorEnvelop.Create(result.Errors));
                }

                var timeTaken = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

                // Track analytics (await to ensure DbContext is available)
                await _searchAnalyticsService.TrackSearchAsync(
                    request.Query,
                    _userContext.UserId,
                    result.Data.TotalHits,
                    timeTaken,
                    request.Filters,
                    "posts",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString(),
                    cancellationToken);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching posts");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Search for tours using advanced Elasticsearch capabilities
        /// </summary>
        /// <param name="request">Search request with query, filters, and pagination</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Search results with facets and highlights</returns>
        [HttpPost("tours")]
        [ProducesResponseType(typeof(SearchResponse<TourSearchDocument>), 200)]
        [ProducesResponseType(typeof(ErrorEnvelop<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchTours([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(ErrorEnvelop.Create(new[] { "Search request is required" }));
                }

                var startTime = DateTime.UtcNow;
                var result = await _elasticsearchService.SearchToursAsync(request, cancellationToken);

                if (result.HasError)
                {
                    return BadRequest(ErrorEnvelop.Create(result.Errors));
                }

                var timeTaken = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

                // Track analytics (await to ensure DbContext is available)
                await _searchAnalyticsService.TrackSearchAsync(
                    request.Query,
                    _userContext.UserId,
                    result.Data.TotalHits,
                    timeTaken,
                    request.Filters,
                    "tours",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString(),
                    cancellationToken);

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tours");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get autocomplete suggestions for search queries
        /// </summary>
        /// <param name="request">Autocomplete request with query text</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of autocomplete suggestions</returns>
        [HttpPost("autocomplete")]
        [ProducesResponseType(typeof(AutocompleteResponse), 200)]
        [ProducesResponseType(typeof(ErrorEnvelop<string>), 400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Autocomplete([FromBody] AutocompleteRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Query))
                {
                    return BadRequest(ErrorEnvelop.Create(new[] { "Query is required" }));
                }

                var result = await _elasticsearchService.AutocompleteAsync(request, cancellationToken);

                if (result.HasError)
                {
                    return BadRequest(ErrorEnvelop.Create(result.Errors));
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting autocomplete suggestions");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Check Elasticsearch health status
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Health check result</returns>
        [HttpGet("health")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> HealthCheck(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _elasticsearchService.HealthCheckAsync(cancellationToken);

                if (result.Data)
                {
                    return Ok(new { status = "healthy", message = "Elasticsearch is connected" });
                }

                return StatusCode(503, new { status = "unhealthy", message = "Elasticsearch is not available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch health check failed");
                return StatusCode(503, new { status = "unhealthy", message = "Elasticsearch is not available" });
            }
        }

        /// <summary>
        /// Re-index all posts (admin only)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of re-indexing operation</returns>
        [HttpPost("admin/reindex/posts")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ReindexPosts(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting bulk re-indexing of posts");
                
                // Ensure index exists
                await _elasticsearchService.CreateIndexAsync("urguide-posts", cancellationToken);
                
                const int batchSize = 1000;
                int skip = 0;
                int totalIndexed = 0;
                
                while (true)
                {
                    // Fetch posts in batches with AsNoTracking for better performance
                    var posts = await _context.Posts
                        .AsNoTracking()
                        .Include(p => p.User)
                        .Include(p => p.Catalog)
                        .OrderBy(p => p.Id)
                        .Skip(skip)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);
                    
                    if (!posts.Any())
                        break;
                    
                    _logger.LogInformation("Processing batch: {Count} posts (total so far: {Total})", posts.Count, totalIndexed + posts.Count);
                    
                    // Convert to search documents
                    var searchDocs = posts.Select(p => SearchDocumentMapper.ToSearchDocument(p)).ToList();
                    
                    // Bulk index
                    var result = await _elasticsearchService.BulkIndexPostsAsync(searchDocs, cancellationToken);
                    
                    if (result.HasError)
                    {
                        _logger.LogError("Failed to bulk index posts batch: {Errors}", string.Join(", ", result.Errors));
                        return StatusCode(500, new { message = "Failed to re-index posts", errors = result.Errors });
                    }
                    
                    totalIndexed += posts.Count;
                    skip += batchSize;
                    
                    // Break if we got fewer results than batch size (last batch)
                    if (posts.Count < batchSize)
                        break;
                }
                
                _logger.LogInformation("Successfully re-indexed {Count} posts", totalIndexed);
                return Ok(new { message = $"Successfully re-indexed {totalIndexed} posts" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error re-indexing posts");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Re-index all tours (admin only)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of re-indexing operation</returns>
        [HttpPost("admin/reindex/tours")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ReindexTours(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting bulk re-indexing of tours");
                
                // Ensure index exists
                await _elasticsearchService.CreateIndexAsync("urguide-tours", cancellationToken);
                
                const int batchSize = 500;
                int skip = 0;
                int totalIndexed = 0;
                
                while (true)
                {
                    // Fetch tours in batches with AsNoTracking for better performance
                    var tours = await _context.Set<UrGuide.Data.Entities.Tour.Tour>()
                        .AsNoTracking()
                        .Include(t => t.Author)
                        .ThenInclude(a => a.ProfileInfo)
                        .Include(t => t.Region)
                        .Include(t => t.Reviews)
                        .Include(t => t.Bookings)
                        .Include(t => t.Reactions)
                        .OrderBy(t => t.TourId)
                        .Skip(skip)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);
                    
                    if (!tours.Any())
                        break;
                    
                    _logger.LogInformation("Processing batch: {Count} tours (total so far: {Total})", tours.Count, totalIndexed + tours.Count);
                    
                    // Convert to search documents
                    var searchDocs = tours.Select(t => SearchDocumentMapper.ToSearchDocument(t)).ToList();
                    
                    // Bulk index
                    var result = await _elasticsearchService.BulkIndexToursAsync(searchDocs, cancellationToken);
                    
                    if (result.HasError)
                    {
                        _logger.LogError("Failed to bulk index tours batch: {Errors}", string.Join(", ", result.Errors));
                        return StatusCode(500, new { message = "Failed to re-index tours", errors = result.Errors });
                    }
                    
                    totalIndexed += tours.Count;
                    skip += batchSize;
                    
                    // Break if we got fewer results than batch size (last batch)
                    if (tours.Count < batchSize)
                        break;
                }
                
                _logger.LogInformation("Successfully re-indexed {Count} tours", totalIndexed);
                return Ok(new { message = $"Successfully re-indexed {totalIndexed} tours" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error re-indexing tours");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
