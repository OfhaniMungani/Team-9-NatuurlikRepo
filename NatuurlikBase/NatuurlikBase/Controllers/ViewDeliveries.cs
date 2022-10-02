using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;

namespace NatuurlikBase.Controllers
{

    public class ViewDeliveries : Controller
    {
        private readonly DatabaseContext _context;
        public ViewDeliveries(DatabaseContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            var databaseContext = _context.Delivery;
            return View(await databaseContext.ToListAsync());
        }
    }
}
