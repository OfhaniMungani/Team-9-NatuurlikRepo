using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly DatabaseContext db;
        private readonly IUnitOfWork _unitOfWork;

        public AboutUsController(DatabaseContext _db, IUnitOfWork unitOfWork)
        {
            db = _db;
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

            var videos = db.Video.ToList();
            return View(videos);
        }
    }
}
