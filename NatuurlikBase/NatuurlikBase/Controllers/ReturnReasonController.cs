﻿#nullable disable
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

[Authorize(Roles = SR.Role_Admin)]
public class ReturnReasonController : Controller
{
    private readonly DatabaseContext db;
    private readonly IUnitOfWork _unitOfWork;
    public ReturnReasonController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        db = context;
        _unitOfWork = unitOfWork;
    }

    // GET: Countries
    public IActionResult Index()
    {
        return View(db.ReturnReason.ToList());
    }

    // GET: Countries/Details/5
    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        ReturnReason country = db.ReturnReason.Find(id);
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
    public async Task<IActionResult> Create([Bind("Id,ReturnReasonName")] ReturnReason returnReason)
    {
        if (ModelState.IsValid)

        {
            if (db.ReturnReason.Any(c => c.ReturnReasonName.Equals(returnReason.ReturnReasonName)))
            {
                ViewBag.ReturnError = "Return reason Already exist in the database.";

            }
            else
            {
                db.ReturnReason.Add(returnReason);

                ViewBag.CountryConfirmation = "Are you sure you want to add a return reason.";
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await db.SaveChangesAsync(userName);

                TempData["success"] = "Return reason created successfully.";
                return RedirectToAction("Index");
            }

        }

        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "invalid.";

        }
        return View(returnReason);
    }

    // GET: Countries/Edit/5
    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        ReturnReason country = db.ReturnReason.Find(id);
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
    public async Task<IActionResult> Edit([Bind("Id,ReturnReasonName")] ReturnReason returnReason)
    {
        if (ModelState.IsValid)

        {

            if (db.ReturnReason.Any(c => c.ReturnReasonName.Equals(returnReason.ReturnReasonName)))
            {
                ViewBag.ReturnError = "Return Reason Already exist in the database.";

            }
            else
            {
                //db.Entry(returnReason).State = EntityState.Modified;
                _unitOfWork.ReturnReason.Update(returnReason);
                TempData["success"] = "Return reason updated successfully.";
                ViewBag.ReturnReasonConfirmation = "Are you sure with your return reason changes.";
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await db.SaveChangesAsync(userName);
                return RedirectToAction("Index");
            }
        }
        return View(returnReason);
    }

    // GET: Countries/Delete/5
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {

            return NotFound();
        }
        ReturnReason returnReason = db.ReturnReason.Find(id);
        if (returnReason == null)
        {
            return NotFound();
        }
        return View(returnReason);

    }

    // POST: Countries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task <IActionResult> DeleteConfirmed(int id)
    {
        ReturnReason country = db.ReturnReason.Find(id);
        db.ReturnReason.Remove(country);
        ViewBag.CountryConfirmation = "Are you sure you want to delete a country.";

        var hasFk = _unitOfWork.ReturnedProduct.GetAll().Any(x => x.Id == id);

        if (!hasFk)
        {
            var obj = _unitOfWork.ReturnReason.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                TempData["AlertMessage"] = "Error occurred while attempting delete";
            }
            _unitOfWork.ReturnReason.Remove(obj);
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
            var userName = fullName.ToString();
            await db.SaveChangesAsync(userName);
            TempData["success"] = "Return Reason Successfully Deleted.";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "Return Reason cannot be deleted since it has a Returned Product associated";
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

