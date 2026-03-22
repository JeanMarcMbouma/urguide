namespace UrGuide.Model.Media
{
    public class ImageProcessingRequest
    {
        public string ImageId { get; set; }
        public string Url { get; set; }
        public bool EnableWatermark { get; set; }
        public bool GenerateThumbnail { get; set; } = true;
        public bool GenerateWebP { get; set; } = true;
        public string CdnProvider { get; set; }
    }
}
