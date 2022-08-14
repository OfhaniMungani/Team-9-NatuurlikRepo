using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NatuurlikBase.Controllers
{
    public class InventoryReportController : Controller
    {
        private readonly DatabaseContext db;
        public InventoryReportController(DatabaseContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {

            IEnumerable<InventoryItem> inventoryItems = db.InventoryItem.Include(b => b.InventoryType).ToList();
            ViewBag.total = inventoryItems.Sum(o => o.QuantityOnHand);
            return View(inventoryItems);
        }
    }
}

