namespace UrGuide.Data.Entities.Users
{
    public class Image
    {
      
        public string Id { get; set; }
        public string ImageUrl { get; set; }

        public static implicit operator string(Image image) => image.ImageUrl;
        public override string ToString() => ImageUrl;
    }
}