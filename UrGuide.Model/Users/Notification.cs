using System;
using System.Collections.Generic;
using System.Text;

namespace UrGuide.Model.Users
{
    public class Notification
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public string AuthorImage { get; set; }
        public string ReferenceLink { get; set; }
        public string Created { get; set; }
        public bool Read { get; set; }
        public bool IsSystem { get; set; }
    }
}
