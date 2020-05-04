using Microsoft.AspNetCore.Identity;

namespace UrGuide.Auth.Entities
{
    public class UrGuideUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsGuide { get; set; }
    }
}
