using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using NatuurlikBase.Models;

namespace NatuurlikBase.ViewModels
{
    public class OrderReviewVMcs
    {
        public OrderReview OrderReview { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> OrdersList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ReviewReasons { get; set; }
    }
}
