using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.WebApp.Models
{
    public class CreatePostModel
    {
        public string Location { get; set; }

        [Required, StringLength(500, MinimumLength = 2)]
        public string Text { get; set; }

        public List<string> Photos { get; set; }

        public string UserId { get; set; }
    }
}
