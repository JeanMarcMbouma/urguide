using System;
using System.Collections.Generic;
using UrGuide.Data.Entities.Event;
using UrGuide.Model.Auditing;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Auditing
{
    static class AuditEventMapper
    {
        private static readonly Dictionary<EventCodes, string> EventDescriptions = new Dictionary<EventCodes, string>
        {
            { EventCodes.Login, "User logged in" },
            { EventCodes.Logout, "User logged out" },
            { EventCodes.Register, "User has registered" },
            { EventCodes.EditPost, "User edited a post {0}" },
            { EventCodes.EditCatalog, "User edited a catalog {0}" },
            { EventCodes.DeletePost, "User deleted a post {0}" },
            { EventCodes.DeleteCatalog, "User deleted a catalog {0}" },
            { EventCodes.DeleteAccount, "User deleted his account" },
            { EventCodes.Maintenance, "A service maintenance has occurred" },
            { EventCodes.CreatePost, "User created a post {0}" },
            { EventCodes.CreateCalalog, "User created a catalog {0}" }
        };

        public static ActivityModel ToActivityModel(AuditEvent source)
        {
            return new ActivityModel
            {
                Event = string.Format(EventDescriptions[source.EventCode], source.ReferenceId),
                When = DateTimeHelper.GetDateTime(source.Created, DateTimeKind.Utc)
            };
        }
    }
}
