using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Diagnostics;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
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

            IEnumerable<Product> RecProducts = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand").Take(4);
            return View(RecProducts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}