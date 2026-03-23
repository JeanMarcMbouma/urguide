using System;

namespace UrGuide.Model.Messages
{
    public class ChatUserStatusDto
    {
        public string UserId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastSeen { get; set; }
    }
}
