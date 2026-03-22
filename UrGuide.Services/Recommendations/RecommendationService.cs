using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrGuide.Data;
using UrGuide.Data.Entities.Recommendations;
using UrGuide.Model.Recommendations;

namespace UrGuide.Services.Recommendations
{
    public class RecommendationService : IRecommendationService
    {
        private readonly UrGuideContext _context;

        public RecommendationService(UrGuideContext context)
        {
            _context = context;
        }

        public async Task<List<TourRecommendationDto>> GetRecommendationsAsync(string userId, int count = 10, double? latitude = null, double? longitude = null)
        {
            var recommendations = new List<TourRecommendationDto>();

            var tours = await _context.Set<Data.Entities.Tour.Tour>()
                .Take(count * 3)
                .ToListAsync();

            foreach (var tour in tours)
            {
                var popularityScore = await CalculatePopularityScore(tour.TourId);
                var contentScore = await CalculateContentScore(userId, tour.TourId);
                var collaborativeScore = await CalculateCollaborativeScore(userId, tour.TourId);

                decimal locationScore = 0m;
                if (latitude.HasValue && longitude.HasValue)
                {
                    locationScore = CalculateLocationScore(tour.TourId, latitude.Value, longitude.Value);
                }

                var totalScore = (popularityScore * 0.25m) + (contentScore * 0.30m) + (collaborativeScore * 0.30m) + (locationScore * 0.15m);

                var algorithm = DetermineTopAlgorithm(popularityScore, contentScore, collaborativeScore, locationScore);

                recommendations.Add(new TourRecommendationDto
                {
                    TourId = tour.TourId,
                    TourTitle = tour.Title,
                    Score = Math.Round(totalScore, 4),
                    Algorithm = algorithm,
                    Reason = GenerateReason(algorithm)
                });
            }

            var topRecommendations = recommendations
                .OrderByDescending(r => r.Score)
                .Take(count)
                .ToList();

            // Log recommendations
            foreach (var rec in topRecommendations)
            {
                _context.Set<RecommendationLog>().Add(new RecommendationLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    TourId = rec.TourId,
                    Score = rec.Score,
                    Algorithm = rec.Algorithm,
                    CreatedAt = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();

            return topRecommendations;
        }

        public async Task<List<TourRecommendationDto>> GetPopularToursAsync(int count = 10, double? latitude = null, double? longitude = null)
        {
            var tours = await _context.Set<Data.Entities.Tour.Tour>()
                .Take(count * 2)
                .ToListAsync();

            var recommendations = new List<TourRecommendationDto>();

            foreach (var tour in tours)
            {
                var score = await CalculatePopularityScore(tour.TourId);

                if (latitude.HasValue && longitude.HasValue)
                {
                    var locationScore = CalculateLocationScore(tour.TourId, latitude.Value, longitude.Value);
                    score = (score * 0.7m) + (locationScore * 0.3m);
                }

                recommendations.Add(new TourRecommendationDto
                {
                    TourId = tour.TourId,
                    TourTitle = tour.Title,
                    Score = Math.Round(score, 4),
                    Algorithm = "popularity",
                    Reason = "Popular among other travelers"
                });
            }

            return recommendations
                .OrderByDescending(r => r.Score)
                .Take(count)
                .ToList();
        }

        public async Task<bool> SetUserPreferencesAsync(string userId, SetPreferencesRequest request)
        {
            var existing = await _context.Set<UserPreference>()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            _context.Set<UserPreference>().RemoveRange(existing);

            foreach (var pref in request.Preferences)
            {
                _context.Set<UserPreference>().Add(new UserPreference
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    PreferenceType = pref.PreferenceType,
                    PreferenceValue = pref.PreferenceValue,
                    Weight = pref.Weight,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserPreferenceDto>> GetUserPreferencesAsync(string userId)
        {
            return await _context.Set<UserPreference>()
                .Where(p => p.UserId == userId)
                .Select(p => new UserPreferenceDto
                {
                    PreferenceType = p.PreferenceType,
                    PreferenceValue = p.PreferenceValue,
                    Weight = p.Weight
                })
                .ToListAsync();
        }

        public async Task<bool> RecordInteractionAsync(string userId, RecordInteractionRequest request)
        {
            _context.Set<TourInteraction>().Add(new TourInteraction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                TourId = request.TourId,
                Type = (InteractionType)request.Type,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ProvideFeedbackAsync(string userId, RecommendationFeedbackRequest request)
        {
            var log = await _context.Set<RecommendationLog>()
                .FirstOrDefaultAsync(l => l.Id == request.RecommendationId && l.UserId == userId);

            if (log == null)
                return false;

            log.WasClicked = request.WasClicked;
            log.WasBooked = request.WasBooked;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RecommendationStatsDto> GetRecommendationStatsAsync()
        {
            var logs = _context.Set<RecommendationLog>();
            var totalCount = await logs.CountAsync();

            if (totalCount == 0)
            {
                return new RecommendationStatsDto
                {
                    TotalRecommendations = 0,
                    ClickThroughRate = 0,
                    ConversionRate = 0,
                    TopAlgorithm = "N/A"
                };
            }

            var clickedCount = await logs.CountAsync(l => l.WasClicked);
            var bookedCount = await logs.CountAsync(l => l.WasBooked);

            var topAlgorithm = await logs
                .GroupBy(l => l.Algorithm)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefaultAsync() ?? "N/A";

            return new RecommendationStatsDto
            {
                TotalRecommendations = totalCount,
                ClickThroughRate = Math.Round((decimal)clickedCount / totalCount * 100, 2),
                ConversionRate = Math.Round((decimal)bookedCount / totalCount * 100, 2),
                TopAlgorithm = topAlgorithm
            };
        }

        private async Task<decimal> CalculatePopularityScore(string tourId)
        {
            var bookingCount = await _context.Set<Data.Entities.Tour.Booking>()
                .CountAsync(b => b.TourId == tourId);

            var reviews = await _context.Set<Data.Entities.Tour.Review>()
                .ToListAsync();

            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

            var bookingScore = Math.Min(bookingCount / 100.0m, 1.0m);
            var ratingScore = (decimal)avgRating / 5.0m;

            return (bookingScore * 0.4m) + (ratingScore * 0.6m);
        }

        private async Task<decimal> CalculateContentScore(string userId, string tourId)
        {
            var preferences = await _context.Set<UserPreference>()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (!preferences.Any())
                return 0.5m;

            var tour = await _context.Set<Data.Entities.Tour.Tour>()
                .FirstOrDefaultAsync(t => t.TourId == tourId);

            if (tour == null)
                return 0m;

            decimal matchScore = 0m;
            decimal totalWeight = 0m;

            foreach (var pref in preferences)
            {
                totalWeight += pref.Weight;

                if (pref.PreferenceType == "category" && tour.Tags != null && tour.Tags.Contains(pref.PreferenceValue, StringComparison.OrdinalIgnoreCase))
                {
                    matchScore += pref.Weight;
                }
                else if (pref.PreferenceType == "location" && tour.RegionId == pref.PreferenceValue)
                {
                    matchScore += pref.Weight;
                }
            }

            return totalWeight > 0 ? matchScore / totalWeight : 0.5m;
        }

        private async Task<decimal> CalculateCollaborativeScore(string userId, string tourId)
        {
            // Find users who booked similar tours
            var userBookedTours = await _context.Set<Data.Entities.Tour.Booking>()
                .Where(b => b.AuthorId == userId)
                .Select(b => b.TourId)
                .ToListAsync();

            if (!userBookedTours.Any())
                return 0m;

            // Find other users who booked the same tours
            var similarUserIds = await _context.Set<Data.Entities.Tour.Booking>()
                .Where(b => userBookedTours.Contains(b.TourId) && b.AuthorId != userId)
                .Select(b => b.AuthorId)
                .Distinct()
                .Take(50)
                .ToListAsync();

            if (!similarUserIds.Any())
                return 0m;

            // Check if similar users booked the target tour
            var similarBookings = await _context.Set<Data.Entities.Tour.Booking>()
                .CountAsync(b => b.TourId == tourId && similarUserIds.Contains(b.AuthorId));

            return Math.Min((decimal)similarBookings / similarUserIds.Count, 1.0m);
        }

        private static decimal CalculateLocationScore(string tourId, double latitude, double longitude)
        {
            // Placeholder: without geo-coordinates on tours, return a neutral score
            return 0.5m;
        }

        private static string DetermineTopAlgorithm(decimal popularity, decimal content, decimal collaborative, decimal location)
        {
            var scores = new Dictionary<string, decimal>
            {
                { "popularity", popularity },
                { "content-based", content },
                { "collaborative", collaborative },
                { "location", location }
            };

            return scores.OrderByDescending(s => s.Value).First().Key;
        }

        private static string GenerateReason(string algorithm)
        {
            return algorithm switch
            {
                "popularity" => "Popular among other travelers",
                "content-based" => "Matches your preferences",
                "collaborative" => "Travelers like you enjoyed this",
                "location" => "Near your location",
                _ => "Recommended for you"
            };
        }
    }
}
