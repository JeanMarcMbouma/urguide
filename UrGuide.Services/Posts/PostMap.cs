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
            CreateMap<PostWrapper, PostModel>()
                .ForMember(x => x.Id, x => x.MapFrom(y => y.Data.Id))
                .ForMember(x => x.Text, x => x.MapFrom(y => y.Data.Text))
                .ForMember(x => x.HasReserved, x => x.MapFrom(y => y.HasUserReserved))
                .ForMember(x => x.HasReacted, x => x.MapFrom(y => y.HasReacted))
                 .ForMember(x => x.ReactionType, x => x.MapFrom(y => y.ReactionType))
                .ForMember(x => x.BidCount, x => x.MapFrom(y => y.BidCount))
                .ForMember(x => x.ItineraryCount, x => x.MapFrom(y => y.ItineraryCount))
                .ForMember(x => x.Description, x => x.MapFrom(y => y.Data.Description))
                .ForMember(x => x.Location, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.GeoLocation))))
                .ForMember(x => x.IsBidOptIn, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.BidOptIn))))
                .ForMember(x => x.Dislikes, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Dislikes))))
                .ForMember(x => x.Likes, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Likes))))
                .ForMember(x => x.Price, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Amount))))
                .ForMember(x => x.Rating, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Rating))))
                .ForMember(x => x.PublicationDate, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.PublicationDate))))
                .ForMember(x => x.Categories, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Categories)).Value.Split(',', StringSplitOptions.RemoveEmptyEntries)))
                .ForMember(x => x.Images, x => x.MapFrom(f => f.Data.Catalog.Images.Select(i => new ImageFileModel
                {
                    Id = i.Id, 
                    ImageBase64 = i.ImageUrl,
                    Name = i.Attributes.FirstOrDefault(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
                })))
                .ForMember(x => x.LastEditDate, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.LastEdit))))
                .ForMember(x => x.Seats, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.AllocatedSeats))))
                .ForMember(x => x.ReservedSeats, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.ReservedSeats))))
                .ForMember(x => x.EndDate, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.DateEnd))))
                .ForMember(x => x.EndTime, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.TimeEnd))))
                .ForMember(x => x.StartDate, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.DateStart))))
                .ForMember(x => x.StartTime, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.TimeStart))))
                .ForMember(x => x.Status, x => x.MapFrom(f => f.Data.IsPastDue ? "Expired" : f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Status))))
                .ForMember(x => x.StartingBid, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.Amount))))
                .ForMember(x => x.LastBid, x => x.MapFrom(f => f.Data.Attributes.FirstOrDefault(a => a.Name == nameof(AttributeTypes.LastBid))))
                .ForMember(x => x.AuthorId, x => x.MapFrom(p => p.Data.User != null ? p.Data.User.Id : Constants.EmptyGuid))
                .ForMember(x => x.Author, x => x.MapFrom(p => p.Data.User != null ? p.Data.User.FullName : Constants.Unknown))
                .ForMember(x => x.AuthorAvatar, x => x.MapFrom(p => p.Data.User != null && p.Data.User.ProfileImage != null ? p.Data.User.ProfileImage.ImageUrl : Constants.UnknownImage));

            CreateMap<Itinerary, ItineraryModel>()
                .ReverseMap();

            CreateMap<BidHistory, BidHistoryModel>()
                .ForMember(x => x.Value, x => x.MapFrom(y => y.Value))
                .ForMember(x => x.Created, x => x.MapFrom(y => y.Created.ToString("dd-MMM-yyyy HH:mm")))
                .ForMember(x => x.Author, x => x.MapFrom(y => y.Author.FullName))
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => y.Author.ProfileImage.ImageUrl));
        }
    }
}
