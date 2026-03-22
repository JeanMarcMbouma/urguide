namespace UrGuide.Model.Reviews
{
    public class ModerationStatsDto
    {
        public int PendingCount { get; set; }
        public int FlaggedCount { get; set; }
        public int ResolvedTodayCount { get; set; }
        public int SpamDetectedCount { get; set; }
    }
}
