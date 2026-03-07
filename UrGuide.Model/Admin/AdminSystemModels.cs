using System;
using System.Collections.Generic;

namespace UrGuide.Model.Admin
{
    public class SystemHealthStatus
    {
        public string OverallStatus { get; set; }
        public DateTime CheckedAt { get; set; }
        public List<ServiceHealthItem> Services { get; set; } = new();
    }

    public class ServiceHealthItem
    {
        public string ServiceName { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public long ResponseTimeMs { get; set; }
    }

    public class AdminAuditLogResponse
    {
        public List<AdminAuditLogItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class AdminAuditLogItem
    {
        public string Id { get; set; }
        public string EventCode { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string ReferenceId { get; set; }
        public DateTime Created { get; set; }
    }

    public class AdminWebhookListResponse
    {
        public List<AdminWebhookItem> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class AdminWebhookItem
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
    }

    public class PlatformSettings
    {
        public bool MaintenanceMode { get; set; }
        public bool RegistrationEnabled { get; set; } = true;
        public bool GuideApplicationsEnabled { get; set; } = true;
        public bool TourBookingEnabled { get; set; } = true;
        public bool PaymentsEnabled { get; set; } = true;
        public bool EmailNotificationsEnabled { get; set; } = true;
        public decimal PlatformFeePercentage { get; set; } = 10.0m;
        public int MaxImagesPerPost { get; set; } = 10;
        public int MinBookingDaysAdvance { get; set; } = 1;
    }

    public class AuditLogFilterParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EventCode { get; set; }
    }
}
