using System;
using System.Collections.Generic;

namespace UrGuide.Data.Entities.Messages
{
    public class ConversationEntity
    {
        public string Id { get; set; }
        public string Participant1Id { get; set; }
        public string Participant1Name { get; set; }
        public string Participant2Id { get; set; }
        public string Participant2Name { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastMessageAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<MessageEntity> Messages { get; set; } = new List<MessageEntity>();
    }
}
