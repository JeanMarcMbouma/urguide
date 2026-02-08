using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Results;
using UrGuide.Model.Users;

namespace UrGuide.Services.Contracts
{
    /// <summary>
    /// Service for managing GDPR-compliant user data exports
    /// </summary>
    public interface IDataExportService
    {
        /// <summary>
        /// Request a new data export for the authenticated user
        /// </summary>
        /// <param name="request">Export request details (format)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the export request details</returns>
        Task<Result<DataExportResponse>> RequestExportAsync(DataExportRequestModel request, CancellationToken cancellationToken);
        
        /// <summary>
        /// Get the status of an export request
        /// </summary>
        /// <param name="requestId">Export request ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the export request details</returns>
        Task<Result<DataExportResponse>> GetExportStatusAsync(string requestId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Download an export file using a secure token
        /// </summary>
        /// <param name="token">Secure download token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result containing the file path and metadata</returns>
        Task<Result<(string FilePath, string FileName, long FileSize)>> DownloadExportAsync(string token, CancellationToken cancellationToken);
        
        /// <summary>
        /// Cancel a pending export request
        /// </summary>
        /// <param name="requestId">Export request ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result<bool>> CancelExportAsync(string requestId, CancellationToken cancellationToken);
        
        /// <summary>
        /// Process pending export requests (background job)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of exports processed</returns>
        Task<int> ProcessPendingExportsAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// Clean up expired exports (background job)
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of exports cleaned up</returns>
        Task<int> CleanupExpiredExportsAsync(CancellationToken cancellationToken);
    }
}
