using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class LoginModel {
        [Required, EmailAddress]
        public string UserName { get; set; }  
        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; set; } 
        public bool Persist { get; set; }
    }
}