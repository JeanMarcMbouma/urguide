using System.Collections.Concurrent;

namespace UrGuide.WebApp.Models
{
    public static class FeedbackResponseStore
    {
        public static ConcurrentDictionary<string, string> Responses { get; } = new();
    }
}
