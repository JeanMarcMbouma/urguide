using AutoMapper;
using System;
using System.Linq;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model.Posts;
using UrGuide.Model.Shared;

namespace UrGuide.Services.Posts
{
    class PostMap : Profile
    {
        public PostMap()
        {
            CreateMap<Post, PostModel>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.Id))
                .ForMember(x => x.Text, x => x.MapFrom(y => y.Text))
                .ForMember(x => x.Description, x => x.MapFrom(y => y.Description))
                .ForMember(x => x.Dislikes, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Dislikes))))
                .ForMember(x => x.Likes, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Likes))))
                .ForMember(x => x.Price, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Amount))))
                .ForMember(x => x.Rating, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Rating))))
                .ForMember(x => x.PublicationDate, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.PublicationDate))))
                .ForMember(x => x.Categories, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Categories)).Value.Split(',', StringSplitOptions.RemoveEmptyEntries)))
                .ForMember(x => x.Images, x => x.MapFrom(f => f.Catalog.Images.Select(i => new ImageFileModel
                {
                    Id = i.Id, 
                    ImageBase64 = i.ImageUrl,
                    Name = i.Attributes.First(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
                })))
                .ForMember(x => x.LastEditDate, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.LastEdit))))
                .ForMember(x => x.Seats, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.AllocatedSeats))))
                .ForMember(x => x.EndDate, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.DateEnd))))
                .ForMember(x => x.StartDate, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.DateStart))))
                .ForMember(x => x.StartTime, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.TimeStart))))
                .ForMember(x => x.Status, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Status))))
                .ForMember(x => x.StartingBid, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Amount))))
                .ForMember(x => x.LastBid, x => x.MapFrom(f => f.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.LastBid))))
                .ForMember(x => x.AuthorId, x => x.MapFrom(p => p.User != null ? p.User.Id : Constants.EmptyGuid))
                .ForMember(x => x.Author, x => x.MapFrom(p => p.User != null ? p.User.FullName : Constants.Unknown))
                .ForMember(x => x.AuthorAvatar, x => x.MapFrom(p => p.User != null && p.User.ProfileImage != null ? p.User.ProfileImage.ImageUrl : Constants.UnknownImage));

            CreateMap<Itinerary, ItineraryModel>()
                .ReverseMap();

            CreateMap<BidHistory, BidHistoryModel>()
                .ForMember(x => x.Value, x => x.MapFrom(y => y.Value))
                .ForMember(x => x.Created, x => x.MapFrom(y => y.Created.ToString("dd-MMM-yyyy")))
                .ForMember(x => x.Author, x => x.MapFrom(y => y.Author.FullName))
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => y.Author.ProfileImage));
        }
    }
}
