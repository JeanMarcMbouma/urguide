namespace UrGuide.Model.Users
{
    public class EmailConfirmationModel
    {
        public string Email { get; set; }
        public string ConfirmationToken { get; set; }
    }
}
