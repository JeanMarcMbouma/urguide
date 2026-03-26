using System;

namespace UrGuide.Data.Entities.Users
{
    public class AccountFreezeRecord
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Reason { get; set; }
        public string FrozenByAdminId { get; set; }
        public DateTime FrozenAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? UnfrozenAt { get; set; }
        public string UnfrozenByAdminId { get; set; }
        public string UnfreezeReason { get; set; }
        public AccountFreezeStatus Status { get; set; }
    }

    public enum AccountFreezeStatus
    {
        Active = 0,
        Expired = 1,
        Unfrozen = 2
    }
}
