using AutoMapper;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.Mapping
{
    class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<API.PostModel, PostItem>()
                .ForMember(x => x.AuthorAvatar, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.AuthorAvatar}"));

            CreateMap<API.PostModel, DiscoverItem>()
                .ForMember(x => x.PostId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.Files, x => x.MapFrom(y => y.Images))
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.AuthorAvatar}"));

            CreateMap<API.ImageFileModel, Model.Shared.ImageFileModel>()
                .ForMember(x => x.ImageBase64, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.ImageBase64}"));

            CreateMap<API.ItineraryModel, Model.Posts.ItineraryModel>();

            CreateMap<API.AuthoredFeedback, Model.Shared.AuthoredFeedback>()
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.AuthorImage}"));

            CreateMap<API.CategoryModel, Model.Lookup.CategoryModel>()
                .ForMember(x => x.ImageUrl, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.ImageUrl}"));

            CreateMap<API.BidHistoryModel, Model.Posts.BidHistoryModel>()
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => $"{GlobalSetting.DefaultEndpoint}/{y.AuthorImage}"));
        }
    }
}
