using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.Models
{
    public class Video
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Video Link")]
        public string VideoUrl { get; set; }
    }
}

