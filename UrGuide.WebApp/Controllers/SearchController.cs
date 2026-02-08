using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Search;
using UrGuide.Services.Abstraction;
using UrGuide.Services.Contracts;

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

        public SearchController(
            IElasticsearchService elasticsearchService,
            ISearchAnalyticsService searchAnalyticsService,
            IUserContext userContext,
            ILogger<SearchController> logger)
        {
            _elasticsearchService = elasticsearchService ?? throw new ArgumentNullException(nameof(elasticsearchService));
            _searchAnalyticsService = searchAnalyticsService ?? throw new ArgumentNullException(nameof(searchAnalyticsService));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Search for posts using advanced Elasticsearch capabilities
        /// </summary>
        /// <param name="request">Search request with query, filters, and pagination</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Search results with facets and highlights</returns>
        [HttpPost("posts")]
        [ProducesResponseType(typeof(SearchResponse<PostSearchDocument>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchPosts([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Search request is required");
                }

                var startTime = DateTime.UtcNow;
                var result = await _elasticsearchService.SearchPostsAsync(request, cancellationToken);

                if (result.HasError)
                {
                    return BadRequest(result.Errors);
                }

                var timeTaken = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

                // Track analytics asynchronously (fire and forget)
                _ = _searchAnalyticsService.TrackSearchAsync(
                    request.Query,
                    _userContext.UserId,
                    result.Value.TotalHits,
                    timeTaken,
                    request.Filters,
                    "posts",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString(),
                    CancellationToken.None);

                return Ok(result.Value);
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
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SearchTours([FromBody] SearchRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Search request is required");
                }

                var startTime = DateTime.UtcNow;
                var result = await _elasticsearchService.SearchToursAsync(request, cancellationToken);

                if (result.HasError)
                {
                    return BadRequest(result.Errors);
                }

                var timeTaken = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

                // Track analytics asynchronously (fire and forget)
                _ = _searchAnalyticsService.TrackSearchAsync(
                    request.Query,
                    _userContext.UserId,
                    result.Value.TotalHits,
                    timeTaken,
                    request.Filters,
                    "tours",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString(),
                    CancellationToken.None);

                return Ok(result.Value);
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
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Autocomplete([FromBody] AutocompleteRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Query))
                {
                    return BadRequest("Query is required");
                }

                var result = await _elasticsearchService.AutocompleteAsync(request, cancellationToken);

                if (result.HasError)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(result.Value);
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

                if (result.Value)
                {
                    return Ok(new { status = "healthy", message = "Elasticsearch is connected" });
                }

                return StatusCode(503, new { status = "unhealthy", message = "Elasticsearch is not available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Elasticsearch health check failed");
                return StatusCode(503, new { status = "unhealthy", message = ex.Message });
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
                // This will be implemented in the data synchronization step
                return Ok(new { message = "Re-indexing started. This is an async operation." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error re-indexing posts");
                return StatusCode(500, "Internal server error");
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
                // This will be implemented in the data synchronization step
                return Ok(new { message = "Re-indexing started. This is an async operation." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error re-indexing tours");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
