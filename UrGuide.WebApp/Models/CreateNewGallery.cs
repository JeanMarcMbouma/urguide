using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Models
{
    public class CreateNewGallery
    { 
        public int Id { get; set; }
       
        public string Title { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public File[] Files { get; set; }

        public string UserId { get; set; }

    }

    public class File
    {
        public int Id { get; set; }

        public string Href { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }

  
}
