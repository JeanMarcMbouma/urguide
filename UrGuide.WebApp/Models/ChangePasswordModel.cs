using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class ChangePasswordModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
        [Required, StringLength(100, MinimumLength = 8), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
    }
}
