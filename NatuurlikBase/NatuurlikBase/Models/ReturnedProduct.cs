using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public class ReturnedProduct
    {
        public int Id { get; set; }

        [RegularExpression("(^[0-9]*$)", ErrorMessage = "This value cannot be negative")]
        public int QuantityReceived { get; set; }
        public DateTime DateLogged { get; set; } = DateTime.Now;


        [Display(Name = "Order Number")]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }

        public int ReturnReasonId { get; set; }
        [ForeignKey("ReturnReasonId")]
        [ValidateNever]
        public ReturnReason ReturnReason { get; set; }
    }
}
