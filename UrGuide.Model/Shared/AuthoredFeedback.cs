using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Model.Shared
{
    public class AuthoredFeedback
    {
        public string Text { get; set; }
        public int Rating { get; set; }
        public string PublicationDate { get; set; }
        public string AuthorId { get; set; }
        public string AuthorImage { get; set; }
        public string AuthorFullName { get; set; }
    }
}
