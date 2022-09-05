using Microsoft.AspNetCore.Mvc;

namespace NatuurlikBase.Controllers
{
    public class OrdersByStatusController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
