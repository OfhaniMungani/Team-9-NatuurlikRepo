#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Controllers;
//[Authorize(Roles = SR.Role_Admin)]
public class CountriesController : Controller
{
    private readonly DatabaseContext db;
    private readonly IUnitOfWork _unitOfWork;

    public CountriesController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        db = context;
        _unitOfWork = unitOfWork;
    }

    // GET: Countries
    public IActionResult Index()
    {
        return View(db.Country.ToList());
    }

    // GET: Countries/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        Country country = db.Country.Find(id);
        if (country == null)
        {
            return NotFound();
        }
        return View(country);
    }

    // GET: Countries/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Countries/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,CountryName")] Country country)
    {
        if (ModelState.IsValid)

        {
            if (db.Country.Any(c => c.CountryName.Equals(country.CountryName)))
            {
                ViewBag.CountryError = "Country name Already exist in the database.";

           
            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                db.Country.Add(country);
                
                ViewBag.CountryConfirmation = "Are you sure you want to add a country.";
                await db.SaveChangesAsync("John");
                TempData["success"] = "Country name created successfully.";
                TempData["NextCreation"] = "Hello World.";
                return RedirectToAction("Index");
            }

        }

        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "invalid.";

        }
        return View(country);
    }

    // GET: Countries/Edit/5
    public async Task <IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        Country country = db.Country.Find(id);
        if (country == null)
        {
            return NotFound();
        }
        return View(country);
    }

    // POST: Countries/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("Id,CountryName")] Country country)
    {
        if (ModelState.IsValid)

        {

            if (db.Country.Any(c => c.CountryName.Equals(country.CountryName)))
            {
                ViewBag.CountryError = "Country name Already exist in the database.";

            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                //db.Entry(country).State = EntityState.Modified;
                _unitOfWork.Country.Update(country);
                TempData["success"] = "Country name updated successfully.";
                ViewBag.CountryConfirmation = "Are you sure with your country name changes.";
                await db.SaveChangesAsync(userName);
                return RedirectToAction("Index");
            }
        }
        return View(country);
    }

    // GET: Countries/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {

            return NotFound();
        }
        Country country = db.Country.Find(id);
        if (country == null)
        {
            return NotFound();
        }
        return View(country);

    }

    // POST: Countries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task <IActionResult> DeleteConfirmed(int id)
    {

        Country country = db.Country.Find(id);
        db.Country.Remove(country);
        ViewBag.CountryConfirmation = "Are you sure you want to delete this country?";

        var hasFk = _unitOfWork.Province.GetAll().Any(x => x.CountryId == id);

        if (!hasFk)
        {
            var obj = _unitOfWork.Country.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                TempData["AlertMessage"] = "Error occurred while attempting delete";
            }

            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = user.FirstName + " " + user.Surname;
            var userName = fullName.ToString();

            _unitOfWork.Country.Remove(obj);
            await db.SaveChangesAsync(userName);
            
            TempData["success"] = "Country name successfully Deleted.";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "Country cannot be deleted since it has a Province associated";
            return RedirectToAction("Index");
        }

       

        //db.SaveChanges();
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
