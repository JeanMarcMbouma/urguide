namespace UrGuide.WebApp.Models
{
    public class Shot
    {
        public long Id { get; set; }

        public string FilePath { get; set; }

        public bool HasPost { get; set; }

        public long GalleryId { get; set; }

        public long PostId { get; set; }

        public string UserId { get; set; }

        public string Description { get; set; }
    }
}
