using Microsoft.EntityFrameworkCore;

namespace UrGuide.Data.Entities.Messages
{
    [Owned]
    public class Link
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
    }
}