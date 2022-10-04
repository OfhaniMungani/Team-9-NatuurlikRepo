using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class ContactUs : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ContactUs(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var hasCart = _unitOfWork.UserCart.GetAll(x => x.ApplicationUserId == claim.Value).FirstOrDefault();
                ViewData["has"] = hasCart;
            }

            return View();
        }
    }
}
