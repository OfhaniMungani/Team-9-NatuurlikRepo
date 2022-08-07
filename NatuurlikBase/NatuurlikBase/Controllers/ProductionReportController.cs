using Microsoft.AspNetCore.Mvc;

namespace NatuurlikBase.Controllers
{
    public class ProductionReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
