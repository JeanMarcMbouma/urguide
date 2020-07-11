using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UrGuide.Model.Shared;

namespace UrGuide.Mobile.Models
{
    public class DiscoverItem
    {
        public string PostId { get; set; }
        public string Author { get; set; }
        public string AuthorImage { get; set; }
        public ObservableRangeCollection<ImageFileModel> Files { get; set; } = new ObservableRangeCollection<ImageFileModel>();
    }
}
