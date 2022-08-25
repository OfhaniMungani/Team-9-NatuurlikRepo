using Microsoft.AspNetCore.Mvc;

namespace NatuurlikBase.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
