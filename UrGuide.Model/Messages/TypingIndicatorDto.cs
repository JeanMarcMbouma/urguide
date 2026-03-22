namespace UrGuide.Model.Messages
{
    public class TypingIndicatorDto
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsTyping { get; set; }
    }
}
