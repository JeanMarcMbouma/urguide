using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using UrGuide.Data.Entities.Event;
using UrGuide.Model.Auditing;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Auditing
{
    class AuditEventMap : Profile
    {
        Dictionary<EventCodes, string> _eventDescriptions = new Dictionary<EventCodes, string>
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

        public AuditEventMap()
        {
            CreateMap<AuditEvent, ActivityModel>()
                .ForMember(x => x.Event, y => y.MapFrom(x => string.Format(_eventDescriptions[x.EventCode], x.ReferenceId)))
                .ForMember(x => x.When, y => y.MapFrom(x => DateTimeHelper.GetDateTime(x.Created, DateTimeKind.Utc)));
        }
    }
}
