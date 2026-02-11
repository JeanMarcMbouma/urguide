namespace UrGuide.Data.Entities.Tour
{
    public class MapPin
    {
        public string MapPinId { get; set; }
        public virtual string ImageUrl { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
