using System;
using System.Threading;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Caching
{
    /// <summary>
    /// Provides Redis-backed distributed caching with the cache-aside pattern.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>Retrieves an item from the cache, or returns the default value if not found.</summary>
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>Stores an item in the cache with an optional expiry.</summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cache-aside: returns the cached item if present; otherwise calls <paramref name="factory"/>,
        /// stores the result, and returns it.
        /// </summary>
        Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

        /// <summary>Removes a single item from the cache.</summary>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all keys that share the given tag prefix.
        /// Pass the prefix without a trailing colon, matching the format used by the
        /// <see cref="CacheKeys"/> helpers (e.g. <c>"tour"</c> or <c>"user"</c>).
        /// </summary>
        Task RemoveByTagAsync(string tag, CancellationToken cancellationToken = default);

        /// <summary>Returns true when the Redis backend is reachable.</summary>
        Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
    }
}
