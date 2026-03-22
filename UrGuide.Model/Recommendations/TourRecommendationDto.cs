namespace UrGuide.Model.Recommendations
{
    public class TourRecommendationDto
    {
        public string TourId { get; set; }
        public string TourTitle { get; set; }
        public decimal Score { get; set; }
        public string Algorithm { get; set; }
        public string Reason { get; set; }
    }
}
