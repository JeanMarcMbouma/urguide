using System;

namespace UrGuide.Data.Entities.Media
{
    public class ProcessedImage
    {
        public string Id { get; set; }
        public string OriginalImageId { get; set; }
        public string OriginalUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string MediumUrl { get; set; }
        public string LargeUrl { get; set; }
        public string WebPUrl { get; set; }
        public string Format { get; set; }
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string CdnUrl { get; set; }
        public bool IsWatermarked { get; set; } = false;
        public string ExifDataJson { get; set; }
        public ImageProcessingStatus Status { get; set; } = ImageProcessingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
    }

    public enum ImageProcessingStatus
    {
        Pending = 0,
        Processing = 1,
        Completed = 2,
        Failed = 3
    }
}
