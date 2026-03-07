using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class FeedbackResponseModel
    {
        [Required]
        [StringLength(2000, MinimumLength = 1)]
        public string Response { get; set; }
    }
}
