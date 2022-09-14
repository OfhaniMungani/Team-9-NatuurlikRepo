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

public class WriteOffReasonController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public WriteOffReasonController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }


    public async Task<IActionResult> Index()
    {
        return View(await _context.WriteOffReason.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var writeOffReason = await _context.WriteOffReason
            .FirstOrDefaultAsync(m => m.Id == id);
        if (writeOffReason == null)
        {
            return NotFound();
        }

        return View(writeOffReason);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] WriteOffReason writeOffReason)
    {
        if (ModelState.IsValid)
        {
            if (_context.WriteOffReason.Any(c => c.Name.Equals(writeOffReason.Name)))
            {
                ViewBag.ReturnError = "Write-Off Reason Already Exists!";
            }
            else
            {
                _context.Add(writeOffReason);
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await _context.SaveChangesAsync(userName);
                TempData["success"] = "Write-Off Reason Added Successflly!";
                return RedirectToAction("Index");
            }

        }
        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "Error! Please Try Again.";

        }
        return View(writeOffReason);
    }

    // GET: WriteOffReason/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var writeOffReason = await _context.WriteOffReason.FindAsync(id);
        if (writeOffReason == null)
        {
            return NotFound();
        }
        return View(writeOffReason);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] WriteOffReason writeOffReason)
    {
        if (ModelState.IsValid)

        {

            if (_context.WriteOffReason.Any(c => c.Name.Equals(writeOffReason.Name)))
            {
                ViewBag.ReturnError = "Write-Off Reason Already Exists!";

            }
            else
            {
                _context.Entry(writeOffReason).State = EntityState.Modified;
                TempData["success"] = "Write-Off Reason Updated Successfully";
                ViewBag.WriteOffReasonConfirmation = "Please confirm your changes";
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await _context.SaveChangesAsync(userName);
                return RedirectToAction("Index");
            }
        }
        return View(writeOffReason);
    }

    // GET: WriteOffReason/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var writeOffReason = await _context.WriteOffReason
            .FirstOrDefaultAsync(m => m.Id == id);
        if (writeOffReason == null)
        {
            return NotFound();
        }

        return View(writeOffReason);
    }

    // POST: WriteOffReason/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        WriteOffReason writeoff = _context.WriteOffReason.Find(id);
        _context.WriteOffReason.Remove(writeoff);
        ViewBag.WriteOffReasonConfirmation = "Are you sure you want to delete this write-off reason?";
        TempData["success"] = "Write-Off Reason Deleted Successfully";
        var claimsId = (ClaimsIdentity)User.Identity;
        var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
        var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
        var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
        var userName = fullName.ToString();
        await _context.SaveChangesAsync(userName);
        return RedirectToAction("Index");
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
