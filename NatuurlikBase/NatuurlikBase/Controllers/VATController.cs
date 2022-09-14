using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;

//[Authorize(Roles = SR.Role_Admin)]

public class VATController : Controller
{

    private readonly DatabaseContext db;
    private readonly IUnitOfWork _uow;

    public VATController(DatabaseContext context, IUnitOfWork uow)
    {
        db = context;
        _uow = uow;
    }

    // GET: Countries
    public IActionResult Index()
    {
        var databaseContext = db.VAT;
        return View(databaseContext.ToList());
    }


    public IActionResult Create()
    {
        //ViewData["InventoryTypeId"] = new SelectList(db.InventoryType, "Id", "InventoryTypeName");

        //ViewData["VATStatus"] = new SelectList(db.InventoryType, "Id", "InventoryTypeName");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,VATPercentage,VATFactor,VATStatus,CreatedDate")] VAT vat)
    {
        if (ModelState.IsValid)
        {
            //Check that only one VAT instance is active at all times and no duplicates exist.
            if (db.VAT.Any(c => c.VATPercentage == vat.VATPercentage))
            {
                ViewBag.Error = "VAT Details Already Exists.";
            }

            else if (db.VAT.Any(c => c.VATStatus == vat.VATStatus && vat.VATStatus == "Active"))
            {
                
                    ViewBag.Error = "Only one VAT instance may be active at all times.";
            }

            else
            {
                var vatFactor = Convert.ToDecimal(vat.VATPercentage / 100.00) ;
                vat.VATFactor = vatFactor;
                db.VAT.Add(vat);
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _uow.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await db.SaveChangesAsync(userName);
                TempData["success"] = "VAT Details Created Successfully.";
                return RedirectToAction("Index");
            }

        }

        return View(vat);
    }


    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        VAT vatFromDb = db.VAT.Find(id);
        if (vatFromDb == null)
        {
            return NotFound();
        }

        return View(vatFromDb);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("Id,VATPercentage,VATFactor,VATStatus,CreatedDate")] VAT vat)
    {
        if (ModelState.IsValid)
        {
            //check if other Status = Active

            if (db.VAT.Any(c => c.VATStatus == "Active" && c.VATStatus == vat.VATStatus))
            {
                ViewBag.Error = "Another VAT instance is already Active.";
            }
           
            else
            {
                var vatFactor = Convert.ToDecimal(vat.VATPercentage / 100.00);
                vat.VATFactor = vatFactor;
                //db.Entry(vat).State = EntityState.Modified;
                _uow.VATRepository.Update(vat);
                TempData["success"] = "VAT Details Updated Successfully.";
                ViewBag.Prompt = "Are you sure you wish to save the changes.";
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _uow.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await db.SaveChangesAsync(userName);
                return RedirectToAction("Index");
            }
        }
        return View(vat);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null)
        {

            return NotFound();
        }
        var vatInstance = db.VAT.FirstOrDefault(m => m.Id == id);


        if (vatInstance == null)
        {
            return NotFound();
        }
        return View(vatInstance);

    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        VAT vatInstance = db.VAT.Find(id);
        db.VAT.Remove(vatInstance);
        ViewBag.Confirmation = "Are you sure you want to proceed with removal?";
        TempData["success"] = "VAT Details Successfully Deleted.";
        var claimsId = (ClaimsIdentity)User.Identity;
        var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
        var userRetrieved = _uow.User.GetFirstOrDefault(x => x.Id == claim.Value);
        var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
        var userName = fullName.ToString();
        await db.SaveChangesAsync(userName);
        return RedirectToAction("Index");
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
