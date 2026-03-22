using System;

namespace UrGuide.Data.Entities.Messages
{
    public class MessageEntity
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }

        public virtual ConversationEntity Conversation { get; set; }
    }
}
