using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Models
{
    public class Gallery
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }
    }
}
