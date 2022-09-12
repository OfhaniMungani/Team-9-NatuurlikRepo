﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Controllers;

//[Authorize(Roles = SR.Role_Admin)]
public class ReviewReasonController : Controller
{
    private readonly DatabaseContext db;

    public ReviewReasonController(DatabaseContext context)
    {
        db = context;
    }

    // GET: Countries
    public IActionResult Index()
    {
        return View(db.ReviewReason.ToList());
    }

    // GET: Countries/Details/5


    // GET: Countries/Create
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,Name")] ReviewReason ReviewReason)
    {
        if (ModelState.IsValid)

        {
            if (db.ReviewReason.Any(c => c.Name.Equals(ReviewReason.Name)))
            {
                ViewBag.Error = "Review Reason Already exist in the database.";


            }
            else
            {
                db.ReviewReason.Add(ReviewReason);

                ViewBag.ReviewReasonConfirmation = "Are you sure you want to add a Review reason.";
                db.SaveChanges();

                TempData["success"] = "Review Reason successfully added.";
                TempData["NextCreation"] = "Review Reason Successfully Deleted.";

                return RedirectToAction("Index");
            }

        }

        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "invalid.";

        }
        return View(ReviewReason);
    }


    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        ReviewReason ReviewReason = db.ReviewReason.Find(id);
        if (ReviewReason == null)
        {
            return NotFound();
        }
        return View(ReviewReason);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind("Id,Name")] ReviewReason ReviewReason)
    {
        if (ModelState.IsValid)

        {

            if (db.ReviewReason.Any(c => c.Name.Equals(ReviewReason.Name)))
            {
                ViewBag.Error = "Review Reason Already exist in the database.";

            }
            else
            {
                db.Entry(ReviewReason).State = EntityState.Modified;
                TempData["success"] = "Review Reason Successfully Edited.";
                ViewBag.ReviewReasonConfirmation = "Are you sure with your Review reason changes.";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
        return View(ReviewReason);
    }


    // GET: ReviewReason/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var ReviewReason = await db.ReviewReason
            .FirstOrDefaultAsync(m => m.Id == id);
        if (ReviewReason == null)
        {
            return NotFound();
        }

        return View(ReviewReason);
    }

    // POST: WriteOffReason/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        ReviewReason ReviewReason = db.ReviewReason.Find(id);
        db.ReviewReason.Remove(ReviewReason);

        ViewBag.ReviewReasonConfirmation = "Are you sure you want to delete this Review Reason";

        var ForeignKey = db.OrderReview.Any(x => x.ReviewReasonId == id);
        if (!ForeignKey)
        {
            var obj = db.OrderReview.FirstOrDefault(x => x.ReviewReasonId == id);
            if (obj == null)
            {
                TempData["AlertMessage"] = "Oops! This ReviewReason cannot be deleted!";
            }
            db.ReviewReason.Remove(ReviewReason);
            TempData["success"] = "Review Reason Deleted Successfully";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "ReviewReason cannot be deleted since it has an Order Review associated";
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


