using UrGuide.Model.Shared;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Feedback
{
    static class FeedbackMapper
    {
        public static AuthoredFeedback ToAuthoredFeedback(Data.Shared.Feedback source)
        {
            return new AuthoredFeedback
            {
                Id = source.Id,
                AuthorFullName = source.Author?.FullName?.ToString(),
                AuthorId = source.Author?.Id,
                AuthorImage = source.Author?.ProfileImage?.ImageUrl,
                Text = source.Text,
                Rating = source.Rating,
                GuideResponse = source.GuideResponse,
                PublicationDate = DateTimeHelper.GetDateTime(source.Created, System.DateTimeKind.Local)
            };
        }
    }
}
