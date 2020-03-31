using Microsoft.AspNetCore.Identity;
using System;

namespace UrGuide.WebApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsGuide { get; set; }

        public string Gender { get; set; }

        public DateTime Birthday { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string Description { get; set; }

        public string Profile { get; set; }

        public DateTime Date { get; set; }
    }
}
