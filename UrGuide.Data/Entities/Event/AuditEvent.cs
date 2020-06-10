using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Data.Entities.Event
{
    public class AuditEvent
    {
        public string Id { get; set; }
        public EventCodes EventCode { get; set; }
        public string UserId { get; set; }
        public string ReferenceId { get; set; }
        public DateTime Created { get; protected set; } = DateTime.UtcNow;
    }
}
