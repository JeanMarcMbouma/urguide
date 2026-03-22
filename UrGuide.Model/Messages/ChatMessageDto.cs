using System;
using System.Collections.Generic;

namespace UrGuide.Model.Messages
{
    public class ChatMessageDto
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public List<FileAttachmentDto> Attachments { get; set; } = new();
    }
}
