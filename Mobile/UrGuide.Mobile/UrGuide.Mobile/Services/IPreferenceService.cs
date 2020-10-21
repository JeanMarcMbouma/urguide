namespace UrGuide.Mobile.Services
{
    public interface IPreferenceService
    {
        string AuthToken { get; set; }
        string FullName { get; set; }
        string UserId { get; set; }
        string Role { get; set; }
        string Image { get; set; }
    }
}