using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Disputes
{
    public class ResolveDisputeRequest
    {
        [Required]
        [StringLength(4000, MinimumLength = 10)]
        public string Resolution { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Refund amount must be non-negative")]
        public decimal? RefundAmount { get; set; }
    }
}
