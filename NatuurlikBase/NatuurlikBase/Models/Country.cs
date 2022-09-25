using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Country Name")]
        [MaxLength(25)]
        [RegularExpression(@"^[a-zA-Z-]+[ ]?([a-zA-Z-]+[ ]?)*$",
         ErrorMessage = "Invalid Country Name: Two consecutive white spaces and digits are not allowed.")]
        public string CountryName { get; set; }
    }
}
