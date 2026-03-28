using FluentAssertions;
using UrGuide.Data.Entities.Recommendations;
using UrGuide.Model.Recommendations;
using UrGuide.Services.Recommendations;

namespace UrGuide.UnitTests.Recommendations;

public class RecommendationServiceTests
{
    // ─── DetermineTopAlgorithm ─────────────────────────────

    [Fact]
    public void DetermineTopAlgorithm_returns_highest_scoring_algorithm()
    {
        var result = RecommendationService.DetermineTopAlgorithm(0.2m, 0.8m, 0.3m, 0.1m);
        result.Should().Be("content-based");
    }

    [Fact]
    public void DetermineTopAlgorithm_returns_popularity_when_highest()
    {
        var result = RecommendationService.DetermineTopAlgorithm(0.9m, 0.1m, 0.2m, 0.0m);
        result.Should().Be("popularity");
    }

    [Fact]
    public void DetermineTopAlgorithm_returns_collaborative_when_highest()
    {
        var result = RecommendationService.DetermineTopAlgorithm(0.1m, 0.2m, 0.9m, 0.0m);
        result.Should().Be("collaborative");
    }

    [Fact]
    public void DetermineTopAlgorithm_returns_location_when_highest()
    {
        var result = RecommendationService.DetermineTopAlgorithm(0.0m, 0.0m, 0.0m, 0.9m);
        result.Should().Be("location");
    }

    [Fact]
    public void DetermineTopAlgorithm_handles_all_zeros()
    {
        var result = RecommendationService.DetermineTopAlgorithm(0m, 0m, 0m, 0m);
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void DetermineTopAlgorithm_handles_equal_scores()
    {
        var result = RecommendationService.DetermineTopAlgorithm(0.5m, 0.5m, 0.5m, 0.5m);
        result.Should().BeOneOf("popularity", "content-based", "collaborative", "location");
    }

    // ─── GenerateReason ────────────────────────────────────

    [Theory]
    [InlineData("popularity", "Popular among other travelers")]
    [InlineData("content-based", "Matches your preferences")]
    [InlineData("collaborative", "Travelers like you enjoyed this")]
    [InlineData("location", "Near your location")]
    [InlineData("unknown", "Recommended for you")]
    [InlineData("", "Recommended for you")]
    public void GenerateReason_returns_correct_text(string algorithm, string expectedReason)
    {
        var result = RecommendationService.GenerateReason(algorithm);
        result.Should().Be(expectedReason);
    }

    // ─── HaversineDistance ─────────────────────────────────

    [Fact]
    public void HaversineDistance_same_point_returns_zero()
    {
        var distance = RecommendationService.HaversineDistance(48.8566, 2.3522, 48.8566, 2.3522);
        distance.Should().BeApproximately(0.0, 0.001);
    }

    [Fact]
    public void HaversineDistance_paris_to_london_is_approximately_340km()
    {
        // Paris (48.8566, 2.3522) to London (51.5074, -0.1278)
        var distance = RecommendationService.HaversineDistance(48.8566, 2.3522, 51.5074, -0.1278);
        distance.Should().BeApproximately(340.0, 15.0); // ~340 km with tolerance
    }

    [Fact]
    public void HaversineDistance_new_york_to_los_angeles_is_approximately_3940km()
    {
        // New York (40.7128, -74.0060) to LA (34.0522, -118.2437)
        var distance = RecommendationService.HaversineDistance(40.7128, -74.0060, 34.0522, -118.2437);
        distance.Should().BeApproximately(3940.0, 50.0);
    }

    [Fact]
    public void HaversineDistance_handles_antipodal_points()
    {
        // North pole to south pole ≈ 20015 km
        var distance = RecommendationService.HaversineDistance(90.0, 0.0, -90.0, 0.0);
        distance.Should().BeApproximately(20015.0, 100.0);
    }

    // ─── CalculateLocationScore ────────────────────────────

    [Fact]
    public void CalculateLocationScore_same_location_returns_near_one()
    {
        var locations = new Dictionary<string, (double Lat, double Lng)>
        {
            { "tour-1", (48.8566, 2.3522) }
        };

        var score = RecommendationService.CalculateLocationScore(locations, "tour-1", 48.8566, 2.3522);
        score.Should().BeApproximately(1.0m, 0.01m);
    }

    [Fact]
    public void CalculateLocationScore_no_geo_data_returns_zero()
    {
        var emptyLocations = new Dictionary<string, (double Lat, double Lng)>();

        var score = RecommendationService.CalculateLocationScore(emptyLocations, "tour-1", 48.8566, 2.3522);
        score.Should().Be(0m);
    }

    [Fact]
    public void CalculateLocationScore_far_away_returns_zero()
    {
        var locations = new Dictionary<string, (double Lat, double Lng)>
        {
            { "tour-1", (34.0522, -118.2437) } // LA
        };

        // Paris to LA: way beyond 100km
        var score = RecommendationService.CalculateLocationScore(locations, "tour-1", 48.8566, 2.3522);
        score.Should().Be(0m);
    }

    [Fact]
    public void CalculateLocationScore_close_location_returns_high_score()
    {
        // Two locations approximately 10km apart
        var locations = new Dictionary<string, (double Lat, double Lng)>
        {
            { "tour-1", (48.8566, 2.3522) } // Central Paris
        };

        // Slightly offset (~10km)
        var score = RecommendationService.CalculateLocationScore(locations, "tour-1", 48.9466, 2.3522);
        score.Should().BeGreaterThan(0.5m);
    }

    [Fact]
    public void CalculateLocationScore_unknown_tour_returns_zero()
    {
        var locations = new Dictionary<string, (double Lat, double Lng)>
        {
            { "tour-1", (48.8566, 2.3522) }
        };

        var score = RecommendationService.CalculateLocationScore(locations, "tour-999", 48.8566, 2.3522);
        score.Should().Be(0m);
    }

    // ─── CalculateInteractionBoost ─────────────────────────

    [Fact]
    public void CalculateInteractionBoost_no_interactions_returns_zero()
    {
        var interactions = new Dictionary<string, List<InteractionType>>();

        var boost = RecommendationService.CalculateInteractionBoost(interactions, "tour-1");
        boost.Should().Be(0m);
    }

    [Fact]
    public void CalculateInteractionBoost_viewed_returns_small_boost()
    {
        var interactions = new Dictionary<string, List<InteractionType>>
        {
            { "tour-1", new List<InteractionType> { InteractionType.Viewed } }
        };

        var boost = RecommendationService.CalculateInteractionBoost(interactions, "tour-1");
        boost.Should().Be(RecommendationService.InteractionViewWeight);
    }

    [Fact]
    public void CalculateInteractionBoost_bookmarked_returns_medium_boost()
    {
        var interactions = new Dictionary<string, List<InteractionType>>
        {
            { "tour-1", new List<InteractionType> { InteractionType.Bookmarked } }
        };

        var boost = RecommendationService.CalculateInteractionBoost(interactions, "tour-1");
        boost.Should().Be(RecommendationService.InteractionBookmarkWeight);
    }

    [Fact]
    public void CalculateInteractionBoost_multiple_interactions_sum_capped_at_one()
    {
        var interactions = new Dictionary<string, List<InteractionType>>
        {
            { "tour-1", new List<InteractionType>
                {
                    InteractionType.Viewed,
                    InteractionType.Bookmarked,
                    InteractionType.Booked,
                    InteractionType.Reviewed,
                    InteractionType.Shared
                }
            }
        };

        var boost = RecommendationService.CalculateInteractionBoost(interactions, "tour-1");
        boost.Should().Be(1.0m); // Capped at 1.0
    }

    [Fact]
    public void CalculateInteractionBoost_different_tour_returns_zero()
    {
        var interactions = new Dictionary<string, List<InteractionType>>
        {
            { "tour-1", new List<InteractionType> { InteractionType.Booked } }
        };

        var boost = RecommendationService.CalculateInteractionBoost(interactions, "tour-2");
        boost.Should().Be(0m);
    }

    // ─── ValidPreferenceTypes ──────────────────────────────

    [Theory]
    [InlineData("category")]
    [InlineData("location")]
    [InlineData("price_range")]
    [InlineData("duration")]
    [InlineData("language")]
    [InlineData("Category")]
    [InlineData("LOCATION")]
    public void ValidPreferenceTypes_contains_expected_types(string type)
    {
        RecommendationService.ValidPreferenceTypes.Should().Contain(type);
    }

    [Theory]
    [InlineData("invalid_type")]
    [InlineData("")]
    [InlineData("categry")] // Typo
    [InlineData("weather")]
    public void ValidPreferenceTypes_rejects_invalid_types(string type)
    {
        RecommendationService.ValidPreferenceTypes.Should().NotContain(type);
    }

    // ─── Constants ─────────────────────────────────────────

    [Fact]
    public void Algorithm_weights_sum_to_one()
    {
        var total = RecommendationService.PopularityWeight
                  + RecommendationService.ContentWeight
                  + RecommendationService.CollaborativeWeight
                  + RecommendationService.LocationWeight;

        total.Should().Be(1.0m);
    }

    [Fact]
    public void Count_bounds_are_reasonable()
    {
        RecommendationService.MinCount.Should().Be(1);
        RecommendationService.MaxCount.Should().Be(50);
    }

    [Fact]
    public void Interaction_weights_are_ordered_by_engagement()
    {
        // Higher engagement = higher weight
        RecommendationService.InteractionViewWeight.Should().BeLessThan(RecommendationService.InteractionBookmarkWeight);
        RecommendationService.InteractionBookmarkWeight.Should().BeLessThan(RecommendationService.InteractionBookedWeight);
        RecommendationService.InteractionSharedWeight.Should().BeLessThan(RecommendationService.InteractionReviewedWeight);
    }

    // ─── DTO / Entity tests ────────────────────────────────

    [Fact]
    public void TourRecommendationDto_has_all_required_properties()
    {
        var dto = new TourRecommendationDto
        {
            TourId = "tour-1",
            TourTitle = "City Walk",
            Score = 0.85m,
            Algorithm = "popularity",
            Reason = "Popular among other travelers"
        };

        dto.TourId.Should().Be("tour-1");
        dto.TourTitle.Should().Be("City Walk");
        dto.Score.Should().Be(0.85m);
        dto.Algorithm.Should().Be("popularity");
        dto.Reason.Should().Be("Popular among other travelers");
    }

    [Fact]
    public void RecommendationStatsDto_handles_zero_state()
    {
        var stats = new RecommendationStatsDto
        {
            TotalRecommendations = 0,
            ClickThroughRate = 0,
            ConversionRate = 0,
            TopAlgorithm = "N/A"
        };

        stats.TotalRecommendations.Should().Be(0);
        stats.ClickThroughRate.Should().Be(0);
        stats.ConversionRate.Should().Be(0);
        stats.TopAlgorithm.Should().Be("N/A");
    }

    [Fact]
    public void UserPreferenceDto_properties_work_correctly()
    {
        var dto = new UserPreferenceDto
        {
            PreferenceType = "category",
            PreferenceValue = "adventure",
            Weight = 2.5m
        };

        dto.PreferenceType.Should().Be("category");
        dto.PreferenceValue.Should().Be("adventure");
        dto.Weight.Should().Be(2.5m);
    }

    [Fact]
    public void RecommendationLog_default_values_are_correct()
    {
        var log = new RecommendationLog();
        log.WasClicked.Should().BeFalse();
        log.WasBooked.Should().BeFalse();
    }

    [Fact]
    public void TourInteraction_type_enum_has_expected_values()
    {
        ((int)InteractionType.Viewed).Should().Be(0);
        ((int)InteractionType.Bookmarked).Should().Be(1);
        ((int)InteractionType.Booked).Should().Be(2);
        ((int)InteractionType.Reviewed).Should().Be(3);
        ((int)InteractionType.Shared).Should().Be(4);
    }

    [Fact]
    public void UserPreference_default_weight_is_one()
    {
        var pref = new UserPreference();
        pref.Weight.Should().Be(1.0m);
    }

    [Fact]
    public void SetPreferencesRequest_defaults_to_empty_list()
    {
        var request = new SetPreferencesRequest();
        request.Preferences.Should().NotBeNull();
        request.Preferences.Should().BeEmpty();
    }

    [Fact]
    public void RecordInteractionRequest_has_required_properties()
    {
        var request = new RecordInteractionRequest
        {
            TourId = "tour-1",
            Type = (int)InteractionType.Viewed
        };

        request.TourId.Should().Be("tour-1");
        request.Type.Should().Be(0);
    }

    [Fact]
    public void RecommendationFeedbackRequest_has_required_properties()
    {
        var request = new RecommendationFeedbackRequest
        {
            RecommendationId = "rec-1",
            WasClicked = true,
            WasBooked = false
        };

        request.RecommendationId.Should().Be("rec-1");
        request.WasClicked.Should().BeTrue();
        request.WasBooked.Should().BeFalse();
    }
}
