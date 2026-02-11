namespace UrGuide.Model.Users
{
    public class CreateNotification
    {
        public string AuthorId { get; set; }
        public bool IsSystem { get; set; }
        public string Content { get; set; }
        public string ReferenceLink { get; set; }
        public string UserId { get; set; }
    }
}
