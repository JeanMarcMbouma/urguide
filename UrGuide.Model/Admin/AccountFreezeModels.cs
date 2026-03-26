using System;
using System.Collections.Generic;

namespace UrGuide.Model.Admin
{
    /// <summary>
    /// Request model for freezing a user account
    /// </summary>
    public class AccountFreezeRequest
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
        public int? DurationDays { get; set; }
    }

    /// <summary>
    /// Request model for unfreezing a user account
    /// </summary>
    public class AccountUnfreezeRequest
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
    }

    /// <summary>
    /// Response model for account freeze information
    /// </summary>
    public class AccountFreezeInfo
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Reason { get; set; }
        public string FrozenByAdminId { get; set; }
        public DateTime FrozenAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime? UnfrozenAt { get; set; }
        public string UnfrozenByAdminId { get; set; }
        public string UnfreezeReason { get; set; }
        public string Status { get; set; }
    }

    /// <summary>
    /// Response for account freeze history
    /// </summary>
    public class AccountFreezeHistoryResponse
    {
        public List<AccountFreezeInfo> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
