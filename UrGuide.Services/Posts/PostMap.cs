using AutoMapper;
using System;
using System.Linq;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model.Posts;
using UrGuide.Model.Shared;
using UrGuide.Services.Helpers;

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
                .ForMember(x => x.BidCount, x => x.MapFrom(y => y.Data.BidCount))
                .ForMember(x => x.ItineraryCount, x => x.MapFrom(y => y.Data.ItineraryCount))
                .ForMember(x => x.Description, x => x.MapFrom(y => y.Data.Description))
                .ForMember(x => x.Location, x => x.MapFrom(f => f.Data.GeoLocation))
                .ForMember(x => x.IsBidOptIn, x => x.MapFrom(f => f.Data.BidEnabled))
                .ForMember(x => x.Dislikes, x => x.MapFrom(f => f.Data.Dislikes))
                .ForMember(x => x.Likes, x => x.MapFrom(f => f.Data.Likes))
                .ForMember(x => x.Price, x => x.MapFrom(f => f.Data.Cost))
                .ForMember(x => x.Rating, x => x.MapFrom(f => f.Data.Rating))
                .ForMember(x => x.PublicationDate, x => x.MapFrom(f => DateTimeHelper.GetDateTime(f.Data.DateOfPublication, DateTimeKind.Local)))
                .ForMember(x => x.Categories, x => x.MapFrom(f => f.Data.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)))
                .ForMember(x => x.Images, x => x.MapFrom(f => f.Data.Catalog.Images.Select(i => new ImageFileModel
                {
                    Id = i.Id, 
                    ImageBase64 = i.ImageUrl,
                    Name = i.Attributes.FirstOrDefault(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
                })))
                .ForMember(x => x.LastEditDate, x => x.MapFrom(f => DateTimeHelper.GetDateTime(f.Data.LastUpdated, DateTimeKind.Local)))
                .ForMember(x => x.Seats, x => x.MapFrom(f => f.Data.AllocatedSeats))
                .ForMember(x => x.ReservedSeats, x => x.MapFrom(f => f.Data.ReservedSeats))
                .ForMember(x => x.EndDate, x => x.MapFrom(f => DateTimeHelper.GetDate(f.Data.EndDate)))
                .ForMember(x => x.EndTime, x => x.MapFrom(f => DateTimeHelper.GetTime(f.Data.EndDate, DateTimeKind.Local)))
                .ForMember(x => x.StartDate, x => x.MapFrom(f => DateTimeHelper.GetDate(f.Data.StartDate)))
                .ForMember(x => x.StartTime, x => x.MapFrom(f => DateTimeHelper.GetTime(f.Data.StartDate, DateTimeKind.Local)))
                .ForMember(x => x.Status, x => x.MapFrom(f => f.Data.IsPastDue ? "Expired" : "Active"))
                .ForMember(x => x.StartingBid, x => x.MapFrom(f => f.Data.Cost))
                .ForMember(x => x.LastBid, x => x.MapFrom(f => f.Data.LastBid))
                .ForMember(x => x.AuthorId, x => x.MapFrom(p => p.Data.User != null ? p.Data.User.Id : Constants.EmptyGuid))
                .ForMember(x => x.Author, x => x.MapFrom(p => p.Data.User != null ? p.Data.User.FullName : Constants.Unknown))
                .ForMember(x => x.AuthorAvatar, x => x.MapFrom(p => p.Data.User != null && p.Data.User.ProfileImage != null ? p.Data.User.ProfileImage.ImageUrl : Constants.UnknownImage));

            CreateMap<Itinerary, ItineraryModel>()
                .ReverseMap();

            CreateMap<BidHistory, BidHistoryModel>()
                .ForMember(x => x.Value, x => x.MapFrom(y => y.Value))
                .ForMember(x => x.Created, x => x.MapFrom(y => DateTimeHelper.GetDateTime(y.Created, DateTimeKind.Local)))
                .ForMember(x => x.Author, x => x.MapFrom(y => y.Author.FullName))
                .ForMember(x => x.AuthorImage, x => x.MapFrom(y => y.Author.ProfileImage.ImageUrl));
        }
    }
}
