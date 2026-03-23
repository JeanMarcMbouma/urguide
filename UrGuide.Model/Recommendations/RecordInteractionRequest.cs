using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Recommendations
{
    public class RecordInteractionRequest
    {
        [Required]
        public string TourId { get; set; }

        [Required]
        public int Type { get; set; }
    }
}
