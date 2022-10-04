using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NatuurlikBase.Models.ViewModels
{
    public class ProductCategoryVM
    {
        [ValidateNever]
        public List<Product> ProductsList { get; set; }
        [ValidateNever]
        public List<ProductCategory> CategoryList { get; set; }
    }
}
