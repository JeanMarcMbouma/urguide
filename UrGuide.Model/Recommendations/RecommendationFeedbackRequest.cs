using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Recommendations
{
    public class RecommendationFeedbackRequest
    {
        [Required]
        public string RecommendationId { get; set; }

        public bool WasClicked { get; set; }
        public bool WasBooked { get; set; }
    }
}
