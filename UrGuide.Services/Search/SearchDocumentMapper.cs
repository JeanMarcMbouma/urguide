using System;
using System.Linq;
using UrGuide.Data.Entities.Posts;
using UrGuide.Data.Entities.Tour;
using UrGuide.Model.Search;

namespace UrGuide.Services.Search
{
    public static class SearchDocumentMapper
    {
        public static PostSearchDocument ToSearchDocument(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            return new PostSearchDocument
            {
                Id = post.Id,
                Text = post.Text,
                Description = post.Description,
                Tags = post.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList() ?? new System.Collections.Generic.List<string>(),
                GeoLocation = post.GeoLocation,
                Location = post.Location != null ? new Nest.GeoLocation(post.Location.Y, post.Location.X) : null,
                Cost = post.Cost,
                Rating = post.Rating,
                Reviews = post.Reviews,
                AllocatedSeats = post.AllocatedSeats,
                ReservedSeats = post.ReservedSeats,
                AvailableSeats = post.AllocatedSeats - post.ReservedSeats,
                StartDate = post.StartDate,
                EndDate = post.EndDate,
                BidEnabled = post.BidEnabled,
                DateOfPublication = post.DateOfPublication,
                LastUpdated = post.LastUpdated,
                UserId = post.User?.Id,
                UserName = post.User?.UserName,
                UserFirstName = post.User?.FirstName,
                UserLastName = post.User?.LastName,
                Likes = post.Likes,
                Dislikes = post.Dislikes
            };
        }

        public static TourSearchDocument ToSearchDocument(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            var totalReviews = tour.Reviews?.Count ?? 0;
            var averageRating = totalReviews > 0
                ? tour.Reviews.Average(r => r.Rating)
                : 0;

            return new TourSearchDocument
            {
                TourId = tour.TourId,
                Title = tour.Title,
                Description = tour.Description,
                Tags = tour.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList() ?? new System.Collections.Generic.List<string>(),
                Seats = tour.Seats,
                CreatedAt = tour.CreatedAt,
                UpdatedAt = tour.UpdatedAt,
                AuthorId = tour.AuthorId,
                AuthorName = tour.Author != null ? $"{tour.Author.FirstName} {tour.Author.LastName}".Trim() : null,
                RegionId = tour.RegionId,
                RegionName = tour.Region?.Name,
                TotalReviews = totalReviews,
                AverageRating = averageRating,
                TotalBookings = tour.Bookings?.Count ?? 0,
                TotalReactions = tour.Reactions?.Count ?? 0
            };
        }
    }
}
