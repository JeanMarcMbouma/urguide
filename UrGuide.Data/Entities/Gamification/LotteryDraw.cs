using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Gamification
{
    public enum LotteryStatus
    {
        Upcoming = 0,
        Open = 1,
        Closed = 2,
        Drawn = 3,
        Cancelled = 4
    }

    public class LotteryDraw
    {
        public string LotteryDrawId { get; set; }
        public string TourId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MaxEntries { get; set; }
        public int WinnerCount { get; set; }
        public LotteryStatus Status { get; set; }
        public DateTime EntryDeadline { get; set; }
        public DateTime DrawDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<LotteryEntry> Entries { get; set; } = new List<LotteryEntry>();
    }
}
