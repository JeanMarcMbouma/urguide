using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Model.Users
{
    public class CreateUserCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
