namespace UrGuide.Model.Recommendations
{
    public class UserPreferenceDto
    {
        public string PreferenceType { get; set; }
        public string PreferenceValue { get; set; }
        public decimal Weight { get; set; }
    }
}
