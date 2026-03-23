using System.ComponentModel.DataAnnotations;

namespace UrGuide.Model.Disputes
{
    public class SubmitEvidenceRequest
    {
        [Required]
        [StringLength(500)]
        public string FileName { get; set; }

        [Required]
        [StringLength(2000)]
        public string FileUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string FileType { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }
    }
}
