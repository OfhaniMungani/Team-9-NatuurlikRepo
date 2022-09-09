using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers;
[Authorize(Roles = SR.Role_Admin)]
public class ConfirmationReminderController : Controller
{

    private readonly DatabaseContext _db;

    public ConfirmationReminderController(DatabaseContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var current = _db.ConfirmationReminder.Where(x => x.IsActive == "True").FirstOrDefault();
        ViewData["Reminder"] = new SelectList(_db.ConfirmationReminder, "Id", "Days");
        return View(current);
    }

    public IActionResult Update(int id)
    {
        if (ModelState.IsValid)
        {
            if (_db.ConfirmationReminder.Any(c => c.IsActive == "True" && c.Id == id))
            {
                TempData["Delete"] = "The settlement time is already active";
            }
            else
            {
                var old = _db.ConfirmationReminder.Where(x => x.IsActive == "True").FirstOrDefault();
                if (old != null)
                {
                    old.IsActive = "False";
                }
                var current = _db.ConfirmationReminder.Where(z => z.Id == id).FirstOrDefault();
                if (current != null)
                {
                    current.IsActive = "True";
                }
                TempData["success"] = "Order Confrimation Time Updated Successfully";
                _db.SaveChanges();

            }
        }
        return RedirectToAction("Index");
    }
}
