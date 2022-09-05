using Microsoft.AspNetCore.Mvc;

namespace NatuurlikBase.Controllers
{
    public class OrdersByRegionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
