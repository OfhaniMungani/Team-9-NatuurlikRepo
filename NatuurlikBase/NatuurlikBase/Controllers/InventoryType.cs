using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;
[Authorize(Roles = SR.Role_Admin)]
public class InventoryTypeController : Controller
{
    private readonly DatabaseContext db;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryTypeController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        db = context;
        _unitOfWork = unitOfWork;
    }

    // GET: Countries
    public IActionResult Index()
    {
        return View(db.InventoryType.ToList());
    }

    // GET: Countries/Details/5


    // GET: Countries/Create
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,InventoryTypeName")] InventoryType inventoryType)
    {
        if (ModelState.IsValid)

        {
            if (db.InventoryType.Any(c => c.InventoryTypeName.Equals(inventoryType.InventoryTypeName)))
            {
                ViewBag.Error = "Inventory Type Already exist in the database.";


            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                db.InventoryType.Add(inventoryType);
                await db.SaveChangesAsync(userName);
                ViewBag.CountryConfirmation = "Are you sure you want to add a return reason.";

                TempData["success"] = "Inventory Type created successfully.";
                TempData["NextCreation"] = "Inventory Type Successfully Deleted.";

                return RedirectToAction("Index");
            }

        }

        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "invalid.";

        }
        return View(inventoryType);
    }


    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        InventoryType inventoryType = db.InventoryType.Find(id);
        if (inventoryType == null)
        {
            return NotFound();
        }
        return View(inventoryType);
    }

  
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("Id,InventoryTypeName")] InventoryType inventoryType)
    {
        if (ModelState.IsValid)

        {

            if (db.InventoryType.Any(c => c.InventoryTypeName.Equals(inventoryType.InventoryTypeName)))
            {
                ViewBag.Error = "Return Reason Already exist in the database.";

            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                //db.Entry(inventoryType).State = EntityState.Modified;
                _unitOfWork.InventoryType.Update(inventoryType);
                ViewBag.ReturnReasonConfirmation = "Are you sure with your return reason changes.";
                await db.SaveChangesAsync(userName);
                TempData["success"] = "Inventory Type Updated Successfully.";
                return RedirectToAction("Index");
            }
        }
        return View(inventoryType);
    }

  
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {

            return NotFound();
        }
        InventoryType inventoryType = db.InventoryType.Find(id);
        if (inventoryType == null)
        {
            return NotFound();
        }
        return View(inventoryType);

    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        InventoryType inventoryType = db.InventoryType.Find(id);
        db.InventoryType.Remove(inventoryType);

        ViewBag.CountryConfirmation = "Are you sure you want to delete a country.";
        var hasFk =db.InventoryItem.Any(c => c.InventoryTypeId == id);

        if (!hasFk)
        {
            var obj = db.InventoryType.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                TempData["AlertMessage"] = "Error occurred while attempting delete";
            }

            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = user.FirstName + " " + user.Surname;
            var userName = fullName.ToString();

            db.InventoryType.Remove(obj);
            await db.SaveChangesAsync(userName);
            TempData["success"] = "Inventory Type Successfully Deleted.";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "Inventory Type cannot be deleted since it has an Inventory Item associated";
            return RedirectToAction("Index");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}

