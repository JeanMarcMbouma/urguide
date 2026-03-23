using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Disputes
{
    public class CreateDisputeRequest
    {
        [Required]
        public string BookingId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(4000, MinimumLength = 20)]
        public string Description { get; set; }

        [Required]
        public int Category { get; set; }
    }
}
