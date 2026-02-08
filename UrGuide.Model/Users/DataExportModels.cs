using System;

namespace UrGuide.Model.Users
{
    /// <summary>
    /// Request model for creating a data export
    /// </summary>
    public class DataExportRequestModel
    {
        /// <summary>
        /// Export format (JSON or CSV)
        /// </summary>
        public DataExportFormat Format { get; set; } = DataExportFormat.Json;
    }
    
    /// <summary>
    /// Response model for data export request
    /// </summary>
    public class DataExportResponse
    {
        /// <summary>
        /// Unique identifier for the export request
        /// </summary>
        public string RequestId { get; set; }
        
        /// <summary>
        /// Current status of the export
        /// </summary>
        public DataExportStatus Status { get; set; }
        
        /// <summary>
        /// Format of the export (JSON or CSV)
        /// </summary>
        public DataExportFormat Format { get; set; }
        
        /// <summary>
        /// When the export was requested
        /// </summary>
        public DateTime RequestedAt { get; set; }
        
        /// <summary>
        /// When the export will expire (7 days from completion)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
        
        /// <summary>
        /// When the export was completed
        /// </summary>
        public DateTime? CompletedAt { get; set; }
        
        /// <summary>
        /// Secure download URL (only available when status is Completed)
        /// </summary>
        public string DownloadUrl { get; set; }
        
        /// <summary>
        /// File size in bytes (only available when completed)
        /// </summary>
        public long? FileSizeBytes { get; set; }
    }
}
