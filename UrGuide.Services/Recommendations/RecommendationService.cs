using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.Data.Entities.Recommendations;
using UrGuide.Model.Recommendations;

namespace UrGuide.Services.Recommendations
{
    public class RecommendationService : IRecommendationService
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<RecommendationService> _logger;

        // Algorithm weight constants
        internal const decimal PopularityWeight = 0.25m;
        internal const decimal ContentWeight = 0.30m;
        internal const decimal CollaborativeWeight = 0.30m;
        internal const decimal LocationWeight = 0.15m;

        // Scoring constants
        internal const decimal BookingScaleWeight = 0.4m;
        internal const decimal RatingScaleWeight = 0.6m;
        internal const int BookingNormalizationFactor = 100;
        internal const decimal MaxRating = 5.0m;
        internal const int MaxSimilarUsers = 50;
        internal const int MinCount = 1;
        internal const int MaxCount = 50;
        internal const decimal DefaultContentScore = 0.5m;

        // Interaction scoring weights (used to boost collaborative signal)
        internal const decimal InteractionViewWeight = 0.1m;
        internal const decimal InteractionBookmarkWeight = 0.3m;
        internal const decimal InteractionBookedWeight = 0.5m;
        internal const decimal InteractionReviewedWeight = 0.4m;
        internal const decimal InteractionSharedWeight = 0.2m;

        // Location scoring constants
        internal const double MaxRelevantDistanceKm = 100.0;
        internal const double EarthRadiusKm = 6371.0;

        // Popular tours blending weights
        internal const decimal PopularTourScoreWeight = 0.7m;
        internal const decimal PopularTourLocationWeight = 0.3m;

        // Preference constraints
        internal const decimal MaxPreferenceWeight = 10.0m;
        internal const int MaxPreferences = 20;

        // Valid preference types
        public static readonly HashSet<string> ValidPreferenceTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "category", "location", "price_range", "duration", "language"
        };

        public RecommendationService(UrGuideContext context, ILogger<RecommendationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<TourRecommendationDto>> GetRecommendationsAsync(string userId, int count = 10, double? latitude = null, double? longitude = null)
        {
            count = Math.Clamp(count, MinCount, MaxCount);

            if (latitude.HasValue)
                latitude = Math.Clamp(latitude.Value, -90.0, 90.0);
            if (longitude.HasValue)
                longitude = Math.Clamp(longitude.Value, -180.0, 180.0);

            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Generating recommendations for user {UserId}, count={Count}, lat={Latitude}, lng={Longitude}",
                userId, count, latitude, longitude);

            // Get user's already-booked tour IDs to exclude from recommendations
            var userBookedTourIds = await _context.Set<Data.Entities.Tour.Booking>()
                .AsNoTracking()
                .Where(b => b.AuthorId == userId)
                .Select(b => b.TourId)
                .ToListAsync();

            var tours = await _context.Set<Data.Entities.Tour.Tour>()
                .AsNoTracking()
                .Where(t => !userBookedTourIds.Contains(t.TourId))
                .Take(count * 3)
                .ToListAsync();

            var tourIds = tours.Select(t => t.TourId).ToList();

            // Preload aggregates to avoid N+1 queries
            var bookingCounts = await _context.Set<Data.Entities.Tour.Booking>()
                .AsNoTracking()
                .Where(b => tourIds.Contains(b.TourId))
                .GroupBy(b => b.TourId)
                .Select(g => new { TourId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.TourId, g => g.Count);

            var tourRatings = await _context.Set<Data.Entities.Tour.Tour>()
                .AsNoTracking()
                .Where(t => tourIds.Contains(t.TourId))
                .Select(t => new
                {
                    t.TourId,
                    AvgRating = t.Reviews.Any() ? t.Reviews.Average(r => (double)r.Rating) : 0.0
                })
                .ToDictionaryAsync(t => t.TourId, t => t.AvgRating);

            var userPreferences = await _context.Set<UserPreference>()
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            // Load tour interactions for the user to boost scores
            // Materialize flat list first, then group in memory to avoid EF Core translation issues
            var userInteractionItems = await _context.Set<TourInteraction>()
                .AsNoTracking()
                .Where(i => i.UserId == userId && tourIds.Contains(i.TourId))
                .Select(i => new { i.TourId, i.Type })
                .Distinct()
                .ToListAsync();

            var userInteractions = userInteractionItems
                .GroupBy(x => x.TourId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Type).Distinct().ToList()
                );

            // Load MapPins for location scoring
            Dictionary<string, (double Lat, double Lng)> tourLocations = new();
            if (latitude.HasValue && longitude.HasValue)
            {
                tourLocations = await _context.Set<Data.Entities.Tour.Tour>()
                    .AsNoTracking()
                    .Where(t => tourIds.Contains(t.TourId))
                    .SelectMany(t => t.MapPins, (t, mp) => new { t.TourId, mp.Latitude, mp.Longitude })
                    .Where(x => x.Latitude.HasValue && x.Longitude.HasValue)
                    .GroupBy(x => x.TourId)
                    .Select(g => new
                    {
                        TourId = g.Key,
                        Lat = g.Average(x => x.Latitude!.Value),
                        Lng = g.Average(x => x.Longitude!.Value)
                    })
                    .ToDictionaryAsync(x => x.TourId, x => (x.Lat, x.Lng));
            }

            var similarUserIds = userBookedTourIds.Any()
                ? await _context.Set<Data.Entities.Tour.Booking>()
                    .AsNoTracking()
                    .Where(b => userBookedTourIds.Contains(b.TourId) && b.AuthorId != userId)
                    .Select(b => b.AuthorId)
                    .Distinct()
                    .Take(MaxSimilarUsers)
                    .ToListAsync()
                : new List<string>();

            var collaborativeBookings = similarUserIds.Any()
                ? await _context.Set<Data.Entities.Tour.Booking>()
                    .AsNoTracking()
                    .Where(b => tourIds.Contains(b.TourId) && similarUserIds.Contains(b.AuthorId))
                    .GroupBy(b => b.TourId)
                    .Select(g => new { TourId = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(g => g.TourId, g => g.Count)
                : new Dictionary<string, int>();

            var recommendations = new List<TourRecommendationDto>();

            foreach (var tour in tours)
            {
                // Popularity score using preloaded data
                var bCount = bookingCounts.GetValueOrDefault(tour.TourId, 0);
                var avgRating = tourRatings.GetValueOrDefault(tour.TourId, 0.0);
                var bookingScore = Math.Min(bCount / (decimal)BookingNormalizationFactor, 1.0m);
                var ratingScore = (decimal)avgRating / MaxRating;
                var popularityScore = (bookingScore * BookingScaleWeight) + (ratingScore * RatingScaleWeight);

                // Content score using preloaded preferences
                decimal contentScore = DefaultContentScore;
                if (userPreferences.Any())
                {
                    decimal matchScore = 0m;
                    decimal totalWeight = 0m;
                    foreach (var pref in userPreferences)
                    {
                        totalWeight += pref.Weight;
                        if (string.Equals(pref.PreferenceType, "category", StringComparison.OrdinalIgnoreCase)
                            && tour.Tags != null
                            && tour.Tags.Contains(pref.PreferenceValue, StringComparison.OrdinalIgnoreCase))
                        {
                            matchScore += pref.Weight;
                        }
                        else if (string.Equals(pref.PreferenceType, "location", StringComparison.OrdinalIgnoreCase)
                                 && string.Equals(tour.RegionId, pref.PreferenceValue, StringComparison.OrdinalIgnoreCase))
                        {
                            matchScore += pref.Weight;
                        }
                    }
                    contentScore = totalWeight > 0 ? matchScore / totalWeight : DefaultContentScore;
                }

                // Interaction-based boost: factor in user's interactions with this tour
                decimal interactionBoost = CalculateInteractionBoost(userInteractions, tour.TourId);

                // Collaborative score using preloaded data
                decimal collaborativeScore = 0m;
                if (similarUserIds.Any())
                {
                    var simBookings = collaborativeBookings.GetValueOrDefault(tour.TourId, 0);
                    collaborativeScore = Math.Min((decimal)simBookings / similarUserIds.Count, 1.0m);
                }
                // Blend interaction boost into collaborative score
                collaborativeScore = Math.Min(collaborativeScore + interactionBoost, 1.0m);

                // Location score using MapPin coordinates
                decimal locationScore = 0m;
                if (latitude.HasValue && longitude.HasValue)
                {
                    locationScore = CalculateLocationScore(tourLocations, tour.TourId, latitude.Value, longitude.Value);
                }

                var totalScore = (popularityScore * PopularityWeight)
                               + (contentScore * ContentWeight)
                               + (collaborativeScore * CollaborativeWeight)
                               + (locationScore * LocationWeight);

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

            // Batch insert recommendation logs
            var logs = topRecommendations.Select(rec => new RecommendationLog
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                TourId = rec.TourId,
                Score = rec.Score,
                Algorithm = rec.Algorithm,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.Set<RecommendationLog>().AddRange(logs);
            await _context.SaveChangesAsync();

            sw.Stop();
            _logger.LogInformation("Generated {Count} recommendations for user {UserId} in {ElapsedMs}ms",
                topRecommendations.Count, userId, sw.ElapsedMilliseconds);

            return topRecommendations;
        }

        public async Task<List<TourRecommendationDto>> GetPopularToursAsync(int count = 10, double? latitude = null, double? longitude = null)
        {
            count = Math.Clamp(count, MinCount, MaxCount);

            if (latitude.HasValue)
                latitude = Math.Clamp(latitude.Value, -90.0, 90.0);
            if (longitude.HasValue)
                longitude = Math.Clamp(longitude.Value, -180.0, 180.0);

            _logger.LogInformation("Fetching {Count} popular tours", count);

            // Order by booking count descending in SQL to surface genuinely popular tours
            var tours = await _context.Set<Data.Entities.Tour.Tour>()
                .AsNoTracking()
                .OrderByDescending(t => t.Bookings.Count)
                .ThenByDescending(t => t.Reviews.Any() ? t.Reviews.Average(r => (double)r.Rating) : 0.0)
                .Take(count * 2)
                .ToListAsync();

            var tourIds = tours.Select(t => t.TourId).ToList();

            // Batch-load booking counts and ratings to avoid N+1
            var bookingCounts = await _context.Set<Data.Entities.Tour.Booking>()
                .AsNoTracking()
                .Where(b => tourIds.Contains(b.TourId))
                .GroupBy(b => b.TourId)
                .Select(g => new { TourId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.TourId, g => g.Count);

            var tourRatings = await _context.Set<Data.Entities.Tour.Tour>()
                .AsNoTracking()
                .Where(t => tourIds.Contains(t.TourId))
                .Select(t => new
                {
                    t.TourId,
                    AvgRating = t.Reviews.Any() ? t.Reviews.Average(r => (double)r.Rating) : 0.0
                })
                .ToDictionaryAsync(t => t.TourId, t => t.AvgRating);

            // Load MapPins for location scoring
            Dictionary<string, (double Lat, double Lng)> tourLocations = new();
            if (latitude.HasValue && longitude.HasValue)
            {
                tourLocations = await _context.Set<Data.Entities.Tour.Tour>()
                    .AsNoTracking()
                    .Where(t => tourIds.Contains(t.TourId))
                    .SelectMany(t => t.MapPins, (t, mp) => new { t.TourId, mp.Latitude, mp.Longitude })
                    .Where(x => x.Latitude.HasValue && x.Longitude.HasValue)
                    .GroupBy(x => x.TourId)
                    .Select(g => new
                    {
                        TourId = g.Key,
                        Lat = g.Average(x => x.Latitude!.Value),
                        Lng = g.Average(x => x.Longitude!.Value)
                    })
                    .ToDictionaryAsync(x => x.TourId, x => (x.Lat, x.Lng));
            }

            var recommendations = new List<TourRecommendationDto>();

            foreach (var tour in tours)
            {
                var bCount = bookingCounts.GetValueOrDefault(tour.TourId, 0);
                var avgRating = tourRatings.GetValueOrDefault(tour.TourId, 0.0);
                var bookingScore = Math.Min(bCount / (decimal)BookingNormalizationFactor, 1.0m);
                var ratingScore = (decimal)avgRating / MaxRating;
                var score = (bookingScore * BookingScaleWeight) + (ratingScore * RatingScaleWeight);

                if (latitude.HasValue && longitude.HasValue)
                {
                    var locationScore = CalculateLocationScore(tourLocations, tour.TourId, latitude.Value, longitude.Value);
                    score = (score * PopularTourScoreWeight) + (locationScore * PopularTourLocationWeight);
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
            if (request?.Preferences == null || !request.Preferences.Any())
            {
                _logger.LogWarning("SetUserPreferences called with empty preferences for user {UserId}", userId);
                return false;
            }

            if (request.Preferences.Count > MaxPreferences)
            {
                _logger.LogWarning(
                    "SetUserPreferences called with {Count} preferences for user {UserId}, which exceeds the maximum of {Max}.",
                    request.Preferences.Count,
                    userId,
                    MaxPreferences);
                return false;
            }

            // Validate preference types
            var invalidTypes = request.Preferences
                .Where(p => !ValidPreferenceTypes.Contains(p.PreferenceType ?? string.Empty))
                .Select(p => p.PreferenceType)
                .ToList();

            if (invalidTypes.Any())
            {
                _logger.LogWarning("Invalid preference types provided for user {UserId}: {InvalidTypes}. Valid types: {ValidTypes}",
                    userId, string.Join(", ", invalidTypes), string.Join(", ", ValidPreferenceTypes));
                return false;
            }

            var existing = await _context.Set<UserPreference>()
                .Where(p => p.UserId == userId)
                .ToListAsync();

            _context.Set<UserPreference>().RemoveRange(existing);

            foreach (var pref in request.Preferences)
            {
                var weight = Math.Clamp(pref.Weight, 0.0m, MaxPreferenceWeight);
                _context.Set<UserPreference>().Add(new UserPreference
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    PreferenceType = pref.PreferenceType.ToLowerInvariant(),
                    PreferenceValue = pref.PreferenceValue,
                    Weight = weight,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated {Count} preferences for user {UserId}", request.Preferences.Count, userId);
            return true;
        }

        public async Task<List<UserPreferenceDto>> GetUserPreferencesAsync(string userId)
        {
            return await _context.Set<UserPreference>()
                .AsNoTracking()
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
            if (!Enum.IsDefined(typeof(InteractionType), request.Type))
            {
                _logger.LogWarning("Invalid interaction type {Type} from user {UserId}", request.Type, userId);
                return false;
            }

            _context.Set<TourInteraction>().Add(new TourInteraction
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                TourId = request.TourId,
                Type = (InteractionType)request.Type,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("Recorded interaction {Type} for user {UserId} on tour {TourId}",
                (InteractionType)request.Type, userId, request.TourId);
            return true;
        }

        public async Task<bool> ProvideFeedbackAsync(string userId, RecommendationFeedbackRequest request)
        {
            var log = await _context.Set<RecommendationLog>()
                .FirstOrDefaultAsync(l => l.Id == request.RecommendationId && l.UserId == userId);

            if (log == null)
            {
                _logger.LogWarning("Recommendation {RecommendationId} not found for user {UserId}", request.RecommendationId, userId);
                return false;
            }

            log.WasClicked = request.WasClicked;
            log.WasBooked = request.WasBooked;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Feedback recorded for recommendation {RecommendationId}: clicked={Clicked}, booked={Booked}",
                request.RecommendationId, request.WasClicked, request.WasBooked);
            return true;
        }

        public async Task<RecommendationStatsDto> GetRecommendationStatsAsync()
        {
            var logs = _context.Set<RecommendationLog>().AsNoTracking();
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

        /// <summary>
        /// Calculate a boost score based on user interactions with a specific tour.
        /// Different interaction types have different weights.
        /// </summary>
        internal static decimal CalculateInteractionBoost(Dictionary<string, List<InteractionType>> userInteractions, string tourId)
        {
            if (!userInteractions.TryGetValue(tourId, out var types) || !types.Any())
                return 0m;

            decimal boost = 0m;
            foreach (var type in types)
            {
                boost += type switch
                {
                    InteractionType.Viewed => InteractionViewWeight,
                    InteractionType.Bookmarked => InteractionBookmarkWeight,
                    InteractionType.Booked => InteractionBookedWeight,
                    InteractionType.Reviewed => InteractionReviewedWeight,
                    InteractionType.Shared => InteractionSharedWeight,
                    _ => 0m
                };
            }

            return Math.Min(boost, 1.0m);
        }

        /// <summary>
        /// Calculate location score using Haversine distance between user coordinates
        /// and the average MapPin coordinates of a tour. Returns a score between 0 and 1,
        /// where 1 means very close and 0 means beyond MaxRelevantDistanceKm.
        /// </summary>
        internal static decimal CalculateLocationScore(Dictionary<string, (double Lat, double Lng)> tourLocations, string tourId, double latitude, double longitude)
        {
            if (!tourLocations.TryGetValue(tourId, out var tourCoords))
                return 0m; // No geo-data available for this tour

            var distanceKm = HaversineDistance(latitude, longitude, tourCoords.Lat, tourCoords.Lng);

            if (distanceKm >= MaxRelevantDistanceKm)
                return 0m;

            // Linear decay: score = 1 - (distance / maxDistance)
            return (decimal)(1.0 - (distanceKm / MaxRelevantDistanceKm));
        }

        /// <summary>
        /// Calculate the great-circle distance between two points using the Haversine formula.
        /// </summary>
        internal static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        internal static string DetermineTopAlgorithm(decimal popularity, decimal content, decimal collaborative, decimal location)
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

        internal static string GenerateReason(string algorithm)
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
