using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class RegistrationModel
    {
        [Required, EmailAddress]
        public string UserName { get; set; }
        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
        //[Required, StringLength(100, MinimumLength = 8), Compare(nameof(Password))]
        //public string ConfirmPassword { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
