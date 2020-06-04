using AutoMapper;
using UrGuide.Model.Shared;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Feedback
{
    class FeedbackMap : Profile
    {
        public FeedbackMap()
        {
            CreateMap<Data.Shared.Feedback, AuthoredFeedback>()
                .ForMember(x => x.AuthorFullName, y => y.MapFrom(x => x.Author.FullName))
                .ForMember(x => x.AuthorId, y => y.MapFrom(x => x.Author.Id))
                .ForMember(x => x.AuthorImage, y => y.MapFrom(x => x.Author.ProfileImage))
                .ForMember(x => x.Text, y => y.MapFrom(x => x.Text))
                .ForMember(x => x.Rating, y => y.MapFrom(x => x.Rating))
                .ForMember(x => x.PublicationDate, y => y.MapFrom(x => DateTimeHelper.GetDateTime(x.Created)));
        }
    }
}
