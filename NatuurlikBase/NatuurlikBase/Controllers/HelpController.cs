using Microsoft.AspNetCore.Mvc;

namespace NatuurlikBase.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
