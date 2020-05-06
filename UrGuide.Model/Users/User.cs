namespace UrGuide.Model.Users
{
    public class User
    {
        public string ProfileImage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDay { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string FaceBook { get; set; }
        public string Google { get; set; }
        public string Rating { get; set; }
        public string Instagram { get; set; }
        public string Description { get; set; }

        public bool IsGuide { get; set; }
        public bool IsPremium { get; set; }
        public string Id { get; set; }
    }
}
