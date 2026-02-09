using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Search;

namespace UrGuide.Services.Contracts
{
    public interface IElasticsearchService
    {
        // Index management
        Task<Result<bool>> CreateIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<Result<bool>> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
        
        // Document indexing
        Task<Result<bool>> IndexPostAsync(PostSearchDocument post, CancellationToken cancellationToken = default);
        Task<Result<bool>> IndexTourAsync(TourSearchDocument tour, CancellationToken cancellationToken = default);
        Task<Result<bool>> BulkIndexPostsAsync(IEnumerable<PostSearchDocument> posts, CancellationToken cancellationToken = default);
        Task<Result<bool>> BulkIndexToursAsync(IEnumerable<TourSearchDocument> tours, CancellationToken cancellationToken = default);
        
        // Document updates
        Task<Result<bool>> UpdatePostAsync(PostSearchDocument post, CancellationToken cancellationToken = default);
        Task<Result<bool>> UpdateTourAsync(TourSearchDocument tour, CancellationToken cancellationToken = default);
        
        // Document deletion
        Task<Result<bool>> DeletePostAsync(string postId, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteTourAsync(string tourId, CancellationToken cancellationToken = default);
        
        // Search operations
        Task<Result<SearchResponse<PostSearchDocument>>> SearchPostsAsync(SearchRequest request, CancellationToken cancellationToken = default);
        Task<Result<SearchResponse<TourSearchDocument>>> SearchToursAsync(SearchRequest request, CancellationToken cancellationToken = default);
        
        // Autocomplete
        Task<Result<AutocompleteResponse>> AutocompleteAsync(AutocompleteRequest request, CancellationToken cancellationToken = default);
        
        // Utility
        Task<Result<bool>> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
