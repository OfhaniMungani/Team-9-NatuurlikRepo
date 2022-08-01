using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace NatuurlikBase.Models
{
    public class OrderReview
    {
        public int Id { get; set; }
        [Required]
        public int? OrderId { get; set; }
        [ValidateNever]
        public Order order { get; set; }

        [Required]
        public int? ReviewReasonId { get; set; }
        [ValidateNever]
        public ReviewReason ReviewReason { get; set; }
        [Required]
        public string Description { get; set; }
       
        //public string? UploadEvidenceUrl { get; set; }
        public int Rating { get; set; }
       // public string? QueryFeedback { get; set; }
        [Required]
        public DateTime LoggedDate { get; set; } = DateTime.Now;
    }
}
