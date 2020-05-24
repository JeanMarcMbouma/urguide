namespace UrGuide.Model.Users
{
    public class CreateGuideModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string BirthDay { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
    }
}
