using Microsoft.AspNetCore.Mvc;

namespace NatuurlikBase.Controllers
{
    public class ContactUs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
