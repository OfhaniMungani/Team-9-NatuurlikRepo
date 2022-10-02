using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NatuurlikBase.Models
{
    public partial class Delivery
    {
        

        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("date", TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Now;
        public int OrderId { get; set; }
        [ValidateNever]
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Column("Image")]
        public string img { get; set; }
    }
}

