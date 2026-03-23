using System;

namespace UrGuide.Data.Entities.Messages
{
    public class FileAttachment
    {
        public string Id { get; set; }
        public string MessageId { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; }

        public virtual MessageEntity Message { get; set; }
    }
}
