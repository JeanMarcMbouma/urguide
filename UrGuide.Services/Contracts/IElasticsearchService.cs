using System.Collections.Generic;
using BbQ.Outcome;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Search;

namespace UrGuide.Services.Contracts
{
    public interface IElasticsearchService
    {
        // Index management
        Task<Outcome<bool>> CreateIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
        
        // Document indexing
        Task<Outcome<bool>> IndexPostAsync(PostSearchDocument post, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> IndexTourAsync(TourSearchDocument tour, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> BulkIndexPostsAsync(IEnumerable<PostSearchDocument> posts, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> BulkIndexToursAsync(IEnumerable<TourSearchDocument> tours, CancellationToken cancellationToken = default);
        
        // Document updates
        Task<Outcome<bool>> UpdatePostAsync(PostSearchDocument post, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> UpdateTourAsync(TourSearchDocument tour, CancellationToken cancellationToken = default);
        
        // Document deletion
        Task<Outcome<bool>> DeletePostAsync(string postId, CancellationToken cancellationToken = default);
        Task<Outcome<bool>> DeleteTourAsync(string tourId, CancellationToken cancellationToken = default);
        
        // Search operations
        Task<Outcome<SearchResponse<PostSearchDocument>>> SearchPostsAsync(SearchRequest request, CancellationToken cancellationToken = default);
        Task<Outcome<SearchResponse<TourSearchDocument>>> SearchToursAsync(SearchRequest request, CancellationToken cancellationToken = default);
        
        // Autocomplete
        Task<Outcome<AutocompleteResponse>> AutocompleteAsync(AutocompleteRequest request, CancellationToken cancellationToken = default);
        
        // Utility
        Task<Outcome<bool>> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
