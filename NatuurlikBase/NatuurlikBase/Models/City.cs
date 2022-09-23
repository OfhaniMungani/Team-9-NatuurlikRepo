using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public class City
    {
        public City()
        {
            this.Suburb = new HashSet<Suburb>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "City Name")]
        [MaxLength(25)]
        [RegularExpression(@"^[a-zA-Z]+[ ]?([a-zA-Z]+[ ]?)*$",
         ErrorMessage = "Invalid City Name: Two consecutive white spaces and digits are not allowed.")]
        public string CityName { get; set; }
        public int ProvinceId { get; set; }
        [ValidateNever]
        [ForeignKey("ProvinceId")]
        public Province Province { get; set; }
        public ICollection<Suburb> Suburb { get; set; }
    }
}
