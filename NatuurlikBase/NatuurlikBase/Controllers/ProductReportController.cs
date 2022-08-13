using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers
{
    public class ProductReportController : Controller
    {
        public class ProductReportControllercs : Controller
        {
            private readonly DatabaseContext db;
            public ProductReportControllercs(DatabaseContext _db)
            {
                db = _db;
            }
            public IActionResult Index()
            {
                IEnumerable<Product> products = db.Products.Include(b => b.Brand).Include(c => c.Category).ToList();
                ViewBag.total = products.Sum(d => d.QuantityOnHand);

                return View(products);
            }
        
        
        }
    }
}
