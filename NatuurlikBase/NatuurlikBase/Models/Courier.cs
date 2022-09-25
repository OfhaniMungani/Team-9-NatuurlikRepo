using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public class Courier
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Courier Name")]
        [MaxLength(25)]
        [RegularExpression(@"^[0-9]*[a-zA-Z]+[ ]?([a-zA-Z0-9_]+[ ]?)*$",
         ErrorMessage = "Invalid Courier Name: Two consecutive white spaces and only digits is not permitted.")]
        public string CourierName { get; set; }

        [Required(ErrorMessage = "Courier Fee is required")]
        [Display(Name = "Courier Fee")]
        [Range(0, 10000)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CourierFee { get; set; }

        [Required]
        [Display(Name = "Estimated Delivery Time")]
        public string EstimatedDeliveryTime { get; set; }
    }
}
