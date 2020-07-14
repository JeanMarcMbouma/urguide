using AutoMapper;
using MvvmHelpers;
using System;
using UrGuide.Mobile.Models;

namespace UrGuide.Mobile.Mapping
{
    class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<API.PostModel, Models.PostItem>()
                .ForMember(x => x.AuthorAvatar, x => x.MapFrom(y => $"{Constants.BaseUrl}/{y.AuthorAvatar}"));

            CreateMap<API.PostModel, DiscoverItem>()
                .ForMember(x => x.PostId, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.Files, x => x.MapFrom(y => y.Images))
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => $"{Constants.BaseUrl}/{y.AuthorAvatar}"));

            CreateMap<API.ImageFileModel, Model.Shared.ImageFileModel>()
                .ForMember(x => x.ImageBase64, x => x.MapFrom(y => $"{Constants.BaseUrl}/{y.ImageBase64}"));

            CreateMap<API.ItineraryModel, Model.Posts.ItineraryModel>();
        }
    }
}
