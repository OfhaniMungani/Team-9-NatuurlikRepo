using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Models;
using NatuurlikBase.Models.ViewModels;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class ProductCatalogue : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProductCatalogue(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? categoryId)
        {

            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var hasCart = _unitOfWork.UserCart.GetAll(x => x.ApplicationUserId == claim.Value).FirstOrDefault();
                ViewData["has"] = hasCart;
            }


            ProductCategoryVM ProductVM = new ProductCategoryVM();
            if (categoryId != null && categoryId > 0)
            {
                ProductVM.ProductsList = _unitOfWork.Product.GetAll(x => x.Category.Id == categoryId, includeProperties: "Category,Brand").ToList();
            }
            else
            {
                //Get all products and return to the view
                ProductVM.ProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand").ToList();

            }
            ProductVM.CategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(ProductVM);

        }

        public IActionResult Item(int productId)
        {
            Cart cartItem = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == productId, includeProperties: "Category,Brand")
            };

            return View(cartItem);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Item(Cart userCart)
        {
            //Get application user
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            userCart.ApplicationUserId = claim.Value;

            if (claim != null)
            {
                var hasCart = _unitOfWork.UserCart.GetAll(x => x.ApplicationUserId == claim.Value).FirstOrDefault();
                ViewData["has"] = hasCart;
            }

            Cart cart = _unitOfWork.UserCart.GetFirstOrDefault(u => u.ProductId == userCart.ProductId && u.ApplicationUserId == claim.Value);
            var prods = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == userCart.ProductId);

            if (prods.QuantityOnHand >= userCart.Count)
            {
                //check for existing cart
                if (cart == null)
                {
                    _unitOfWork.UserCart.Add(userCart);
                }
                else
                {
                    _unitOfWork.UserCart.increaseCount(cart, userCart.Count);
                }
            }
            else
            {
                //DO Something
                TempData["success"] = "Requested quantity exceeds available stock on hand.";
            }

            //Save cart changes to database
            _unitOfWork.Save();
            //Redirect to Product Catalogue Index page if saved successfully.
            return RedirectToAction("Index", "UserCart");
        }

    }
}
