using System;
using System.Collections.Generic;
using System.Linq;
using UrGuide.Data.Entities.Posts;
using UrGuide.Model.Posts;
using UrGuide.Model.Shared;
using UrGuide.Services.Helpers;

namespace UrGuide.Services.Posts
{
    static class PostMapper
    {
        public static PostModel ToPostModel(PostWrapper source)
        {
            var model = new PostModel
            {
                Id = source.Data.Id,
                Text = source.Data.Text,
                HasReserved = source.HasUserReserved,
                HasReacted = source.HasReacted,
                ReactionType = (int)source.ReactionType,
                BidCount = source.Data.BidCount,
                ItineraryCount = source.Data.ItineraryCount,
                Description = source.Data.Description,
                Location = source.Data.GeoLocation,
                IsBidOptIn = source.Data.BidEnabled,
                Dislikes = source.Data.Dislikes,
                Likes = source.Data.Likes,
                Price = source.Data.Cost,
                Rating = source.Data.Rating.ToString(),
                PublicationDate = DateTimeHelper.GetDateTime(source.Data.DateOfPublication, DateTimeKind.Local),
                Itineraries = source.Data.Itineraries.Select(ToItineraryModel).ToList(),
                LastEditDate = DateTimeHelper.GetDateTime(source.Data.LastUpdated, DateTimeKind.Local),
                Seats = source.Data.AllocatedSeats,
                ReservedSeats = source.Data.ReservedSeats,
                EndDate = DateTimeHelper.GetDate(source.Data.EndDate),
                EndTime = DateTimeHelper.GetTime(source.Data.EndDate, DateTimeKind.Local),
                StartDate = DateTimeHelper.GetDate(source.Data.StartDate),
                StartTime = DateTimeHelper.GetTime(source.Data.StartDate, DateTimeKind.Local),
                Status = source.Data.IsPastDue ? "Expired" : "Active",
                StartingBid = source.Data.Cost,
                LastBid = source.Data.LastBid,
                AuthorId = source.Data.User != null ? source.Data.User.Id : Constants.EmptyGuid,
                Author = source.Data.User != null ? source.Data.User.FullName?.ToString() : Constants.Unknown,
                AuthorAvatar = source.Data.User?.ProfileImage != null ? source.Data.User.ProfileImage.ImageUrl : Constants.UnknownImage,
                Reviews = source.Data.Reviews
            };

            foreach (var category in source.Data.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries))
                model.Categories.Add(category);

            if (source.Data.Catalog != null)
            {
                foreach (var image in source.Data.Catalog.Images)
                {
                    model.Images.Add(new ImageFileModel
                    {
                        Id = image.Id,
                        ImageBase64 = image.ImageUrl,
                        Name = image.Attributes.FirstOrDefault(a => a.Name == nameof(Model.Catalogs.CreateImageCatalogModel.Name))
                    });
                }
            }

            return model;
        }

        public static ItineraryModel ToItineraryModel(Itinerary source)
        {
            return new ItineraryModel
            {
                Title = source.Title,
                Description = source.Description,
                Ordinal = source.Ordinal
            };
        }

        public static Itinerary ToItinerary(ItineraryModel source)
        {
            return new Itinerary
            {
                Title = source.Title,
                Description = source.Description,
                Ordinal = source.Ordinal
            };
        }

        public static BidHistoryModel ToBidHistoryModel(BidHistory source)
        {
            return new BidHistoryModel
            {
                Value = source.Value,
                Created = DateTimeHelper.GetDateTime(source.Created, DateTimeKind.Local),
                Author = source.Author?.FullName?.ToString(),
                AuthorImage = source.Author?.ProfileImage?.ImageUrl
            };
        }
    }
}
