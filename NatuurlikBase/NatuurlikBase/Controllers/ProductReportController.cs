using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;
//[Authorize(Roles = SR.Role_Admin + "," + SR.Role_IM)]
public class ProductReportController : Controller
{
        private readonly DatabaseContext db;
        public ProductReportController(DatabaseContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {

            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var actorName = db.Users.Where(x => x.Id == claim.Value).FirstOrDefault();

            ViewBag.ActorName = actorName.FirstName;
            ViewBag.Surname = actorName.Surname;

            IEnumerable<Product> products = db.Products.Include(b => b.Brand).Include(c => c.Category).ToList();
            ViewBag.total = products.Sum(d => d.QuantityOnHand);

            return View(products);
        }
    
    
    }
