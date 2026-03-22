namespace UrGuide.Model.Disputes
{
    public class DisputeStatsDto
    {
        public int OpenCount { get; set; }
        public int UnderReviewCount { get; set; }
        public int ResolvedCount { get; set; }
        public double AverageResolutionDays { get; set; }
    }
}
