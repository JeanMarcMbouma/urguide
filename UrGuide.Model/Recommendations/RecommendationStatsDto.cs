namespace UrGuide.Model.Recommendations
{
    public class RecommendationStatsDto
    {
        public int TotalRecommendations { get; set; }
        public decimal ClickThroughRate { get; set; }
        public decimal ConversionRate { get; set; }
        public string TopAlgorithm { get; set; }
    }
}
