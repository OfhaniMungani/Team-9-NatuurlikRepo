using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;

namespace NatuurlikBase.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly DatabaseContext db;
        public AboutUsController(DatabaseContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var videos = db.Video.ToList();
            return View(videos);
        }
    }
}
