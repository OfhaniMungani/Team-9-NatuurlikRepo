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

public class SuburbsController : Controller
{
    private readonly DatabaseContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public SuburbsController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    // GET: Suburbs
    public async Task<IActionResult> Index()
    {
        var databaseContext = _context.Suburb.Include(s => s.City);
        return View(await databaseContext.ToListAsync());
    }

    // GET: Suburbs/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var suburb = await _context.Suburb
            .Include(s => s.City)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (suburb == null)
        {
            return NotFound();
        }

        return View(suburb);
    }

    // GET: Suburbs/Create
    public IActionResult Create()
    {
        ViewData["CityId"] = new SelectList(_context.City, "Id", "CityName");
        return View();
    }

    // POST: Suburbs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,SuburbName,PostalCode,CityId")] Suburb suburb)
    {
        if (ModelState.IsValid)
            
        {
            if (_context.Suburb.Any(c => c.SuburbName==suburb.SuburbName && c.PostalCode==suburb.PostalCode && c.CityId==suburb.CityId))
            {
                ViewBag.Error = "Suburb Already Exist In The Database.";
                ViewData["CityId"] = new SelectList(_context.Province, "Id", "CountryName", suburb.CityId);
            }
            else
            {
                _context.Add(suburb);
                TempData["success"] = "Suburb Created Successfully";

                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await _context.SaveChangesAsync(userName);
                return RedirectToAction(nameof(Index));
            }
        }
        ViewData["CityId"] = new SelectList(_context.City, "Id", "CityName", suburb.CityId);
        return View(suburb);
    }

    // GET: Suburbs/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var suburb = await _context.Suburb.FindAsync(id);
        if (suburb == null)
        {
            return NotFound();
        }
        ViewData["CityId"] = new SelectList(_context.City, "Id", "CityName", suburb.CityId);
        return View(suburb);
    }

    // POST: Suburbs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,SuburbName,PostalCode,CityId")] Suburb suburb)
    {
        if (id != suburb.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            if (_context.Suburb.Any(c => c.SuburbName == suburb.SuburbName && c.PostalCode == suburb.PostalCode && c.CityId == suburb.CityId))
            {
                ViewBag.Error = "Suburb Already Exist In The Database.";
                ViewData["CityId"] = new SelectList(_context.Province, "Id", "CountryName", suburb.CityId);
            }
            else
            {
                try
                {
                    _unitOfWork.Suburb.Update(suburb);
                    TempData["success"] = "Suburb Updated Successfully";
                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                    var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                    var userName = fullName.ToString();
                    await _context.SaveChangesAsync(userName);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuburbExists(suburb.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
        }
        ViewData["CityId"] = new SelectList(_context.City, "Id", "CityName", suburb.CityId);
        return View(suburb);
    }

    // GET: Suburbs/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var suburb = await _context.Suburb
            .Include(s => s.City)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (suburb == null)
        {
            return NotFound();
        }

        return View(suburb);
    }

    // POST: Suburbs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var suburb = await _context.Suburb.FindAsync(id);
        _context.Suburb.Remove(suburb);

        //Check if Suburb is associated with an Order
        var orderFK = _unitOfWork.Order.GetAll().Any(x => x.SuburbId == id);

        //Check if Suburb is associated with a User
        var userFK = _unitOfWork.User.GetAll().Any(x => x.SuburbId == id);

        //Check if Suburb is associated with a Supplier
        var supplierFK = _unitOfWork.Supplier.GetAll().Any(x => x.SuburbId == id);

        if(orderFK)
        {
            TempData["Delete"] = "Suburb cannot be deleted since it is associated to an Order";
            return RedirectToAction("Index");
        }

        if (userFK)
        {
            TempData["Delete"] = "Suburb cannot be deleted since it is associated to a User";
            return RedirectToAction("Index");
        }

        if (supplierFK)
        {
            TempData["Delete"] = "Suburb cannot be deleted since it is associated to a Supplier";
            return RedirectToAction("Index");
        }

        else
        {
            TempData["success"] = "Suburb Deleted Successfully";
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
            var userName = fullName.ToString();
            await _context.SaveChangesAsync(userName);
            return RedirectToAction(nameof(Index));
        }
    }

    private bool SuburbExists(int id)
    {
        return _context.Suburb.Any(e => e.Id == id);
    }
}
