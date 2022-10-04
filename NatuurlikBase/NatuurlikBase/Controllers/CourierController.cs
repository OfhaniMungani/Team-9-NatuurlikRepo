using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;
[Authorize(Roles = SR.Role_Admin)]
public class CourierController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public CourierController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }


    public async Task<IActionResult> Index()
    {
        return View(await _context.Courier.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courier = await _context.Courier
            .FirstOrDefaultAsync(m => m.Id == id);
        if (courier == null)
        {
            return NotFound();
        }

        return View(courier);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CourierName,CourierFee,EstimatedDeliveryTime")] Courier courier)
    {
        if (ModelState.IsValid)
        {
            if (_context.Courier.Any(c => c.CourierName == courier.CourierName && c.CourierFee == courier.CourierFee && c.EstimatedDeliveryTime == courier.EstimatedDeliveryTime))
            {
                ViewBag.ReturnError = "Courier Already Exists!";
            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                _context.Add(courier);
                await _context.SaveChangesAsync(userName);
                TempData["success"] = "Courier Added Successfully!";
                return RedirectToAction("Index");
            }

        }
        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "Error! Please Try Again.";

        }
        return View(courier);
    }

    // GET: WriteOffReason/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courier = await _context.Courier.FindAsync(id);
        if (courier == null)
        {
            return NotFound();
        }
        return View(courier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CourierName,CourierFee,EstimatedDeliveryTime")] Courier courier)
    {
        if (ModelState.IsValid)

        {

            if (_context.Courier.Any(c => c.CourierName == courier.CourierName && c.CourierFee == courier.CourierFee && c.EstimatedDeliveryTime == courier.EstimatedDeliveryTime))
            {
                ViewBag.ReturnError = "Courier Already Exists!";

            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                _context.Entry(courier).State = EntityState.Modified;
                
                ViewBag.CourierConfirmation = "Please confirm your changes";
                await _context.SaveChangesAsync(userName);
                TempData["success"] = "Courier Updated Successfully";
                return RedirectToAction("Index");
            }
        }
        return View(courier);
    }

    // GET: Courier/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var courier = await _context.Courier
            .FirstOrDefaultAsync(m => m.Id == id);
        if (courier == null)
        {
            return NotFound();
        }

        return View(courier);
    }

    // POST: WriteOffReason/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        Courier courier = _context.Courier.Find(id);
        _context.Courier.Remove(courier);

        ViewBag.CourierConfirmation = "Are you sure you want to delete this courier";

        var ForeignKey = _context.Order.Any(x => x.CourierId == id);
        if (!ForeignKey)
        {
            var obj = _context.Order.FirstOrDefault(x => x.CourierId == id);
            if (obj == null)
            {
                TempData["AlertMessage"] = "Oops! This courier cannot be deleted!";
            }

            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = user.FirstName + " " + user.Surname;
            var userName = fullName.ToString();

            _context.Courier.Remove(courier);
            await _context.SaveChangesAsync(userName);
            TempData["success"] = "Courier Deleted Successfully";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "Courier cannot be deleted since it has an Order associated";
            return RedirectToAction("Index");
        }


    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
        base.Dispose(disposing);
    }
}
