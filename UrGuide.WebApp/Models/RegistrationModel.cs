using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Models
{
    public class RegistrationModel
    {
        [Required, EmailAddress]
        public string UserName { get; set; }
        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
        [Required, StringLength(100, MinimumLength = 8), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
