using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Disputes
{
    public class DisputeMessageRequest
    {
        [Required]
        [StringLength(4000, MinimumLength = 1)]
        public string Content { get; set; }
    }
}
