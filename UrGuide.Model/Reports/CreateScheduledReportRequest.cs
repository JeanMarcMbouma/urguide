using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Reports
{
    public class CreateScheduledReportRequest
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        public int Format { get; set; }

        [Required]
        public int Frequency { get; set; }

        [Required]
        public string EmailRecipients { get; set; }

        public string Parameters { get; set; }
    }
}
