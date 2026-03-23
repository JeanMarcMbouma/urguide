using System;
using System.Collections.Generic;

namespace UrGuide.Model.Media
{
    public class ImageProcessingResult
    {
        public string ImageId { get; set; }
        public string OriginalUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string MediumUrl { get; set; }
        public string LargeUrl { get; set; }
        public string WebPUrl { get; set; }
        public string CdnUrl { get; set; }
        public string Format { get; set; }
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsWatermarked { get; set; }
        public Dictionary<string, string> ExifData { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
