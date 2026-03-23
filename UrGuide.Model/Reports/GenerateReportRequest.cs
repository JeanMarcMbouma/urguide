using System;
using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Reports
{
    public class GenerateReportRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int Format { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Filters { get; set; }
    }
}
