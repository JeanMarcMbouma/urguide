using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Model.Users
{
    public class UserInfo
    {
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public int Rating { get; set; }
        public string ProfileImage { get; set; }
        public string Id { get; set; }
    }
}
