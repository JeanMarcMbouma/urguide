using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Recommendations
{
    public class SetPreferencesRequest
    {
        [Required]
        public List<UserPreferenceDto> Preferences { get; set; } = new List<UserPreferenceDto>();
    }
}
