using System;

namespace UrGuide.Data.Entities.Gamification
{
    public class LotteryEntry
    {
        public string LotteryEntryId { get; set; }
        public string LotteryDrawId { get; set; }
        public virtual LotteryDraw LotteryDraw { get; set; }
        public string UserId { get; set; }
        public bool IsWinner { get; set; }
        public DateTime EnteredAt { get; set; }
    }
}
