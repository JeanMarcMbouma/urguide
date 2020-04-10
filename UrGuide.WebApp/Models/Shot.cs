using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrGuide.WebApp.Models
{
    public class Shot
    {
        public long Id { get; set; }

        public string Photo { get; set; }

        public bool HasPost { get; set; }

        public long GalleryId { get; set; }

        public long PostId { get; set; }

        public string UserId { get; set; }
    }
}
