namespace UrGuide.Model.Users
{
    public class LoginCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Persist { get; set; }
    }
}
