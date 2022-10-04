using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.Models
{
    public class InventoryType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Inventory Type Name")]
        [MaxLength(50)]
        [RegularExpression(@"^[0-9]*[a-zA-Z]+[ ]?([a-zA-Z0-9_]+[ ]?)*$",
         ErrorMessage = "Invalid Inventory Type Name: Two consecutive white spaces and only digits is not permitted.")]
        public string InventoryTypeName { get; set; }
    }
}
