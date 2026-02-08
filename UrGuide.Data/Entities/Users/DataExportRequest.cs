using System;
using UrGuide.Model.Users;

namespace UrGuide.Data.Entities.Users
{
    public class DataExportRequest
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public DataExportStatus Status { get; set; }
        public DataExportFormat Format { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string DownloadToken { get; set; }
        public string FilePath { get; set; }
        public long? FileSizeBytes { get; set; }
    }
}
