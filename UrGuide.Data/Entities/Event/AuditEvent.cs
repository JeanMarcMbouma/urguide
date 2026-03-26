using System;

namespace UrGuide.Data.Entities.Event
{
    public class AuditEvent
    {
        public string Id { get; set; }
        public EventCodes EventCode { get; set; }
        public string UserId { get; set; }
        public string ReferenceId { get; set; }
        public DateTime Created { get; protected set; } = DateTime.UtcNow;
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Details { get; set; }
        public string Category { get; set; }
        public AuditSeverity Severity { get; set; }
    }

    public enum AuditSeverity
    {
        Info = 0,
        Warning = 1,
        Critical = 2
    }
}
