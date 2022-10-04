using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;

[Authorize(Roles = SR.Role_Admin)]
public class QueryReasonController : Controller
{
    private readonly DatabaseContext db;
    private readonly IUnitOfWork _uow;

    public QueryReasonController(DatabaseContext context, IUnitOfWork unitOfWork)
    {
        db = context;
        _uow = unitOfWork;
    }

    // GET: Countries
    public IActionResult Index()
    {
        return View(db.QueryReason.ToList());
    }

    // GET: Countries/Details/5

    public IActionResult Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        QueryReason QueryReason = db.QueryReason.Find(id);
        if (QueryReason == null)
        {
            return NotFound();
        }
        return View(QueryReason);
    }

    // GET: Countries/Create
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name")] QueryReason queryReason)
    {
        if (ModelState.IsValid)

        {
            if (db.QueryReason.Any(c => c.Name.Equals(queryReason.Name)))
            {
                ViewBag.ReturnError = "Query Reason Already exist in the database.";


            }
            else
            {
                db.QueryReason.Add(queryReason);

                ViewBag.QueryReasonConfirmation = "Are you sure you want to add a Query reason.";
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _uow.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await db.SaveChangesAsync(userName);

                TempData["success"] = "Query Reason created successfully.";
                return RedirectToAction("Index");
            }

        }

        else if (!ModelState.IsValid)

        {
            ViewBag.modal = "invalid.";

        }
        return View(queryReason);
    }


    public IActionResult Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        QueryReason queryReason = db.QueryReason.Find(id);
        if (queryReason == null)
        {
            return NotFound();
        }
        return View(queryReason);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([Bind("Id,Name")] QueryReason queryReason)
    {
        if (ModelState.IsValid)

        {

            if (db.QueryReason.Any(c => c.Name.Equals(queryReason.Name)))
            {
                ViewBag.Error = "Query Reason Already exist in the database.";

            }
            else
            {
                //db.Entry(QueryReason).State = EntityState.Modified;
                _uow.QueryReason.Update(queryReason);
                TempData["success"] = "Query Reason Successfully Edited.";
                ViewBag.QueryReasonConfirmation = "Are you sure with your Query reason changes.";
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _uow.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await db.SaveChangesAsync(userName);
                return RedirectToAction("Index");
            }
        }
        return View(queryReason);
    }


    // GET: QueryReason/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        QueryReason queryReason = db.QueryReason.Find(id);
        if (queryReason == null)
        {
            return NotFound();
        }

        return View(queryReason);
    }

    // POST: WriteOffReason/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        QueryReason queryReason = db.QueryReason.Find(id);
        db.QueryReason.Remove(queryReason);

        ViewBag.QueryReasonConfirmation = "Are you sure you want to delete this Query Reason";

        var ForeignKey = db.OrderQuery.Any(x => x.QueryReasonId == id);
        if (!ForeignKey)
        {
            var obj = db.OrderQuery.FirstOrDefault(x => x.QueryReasonId == id);
            if (obj == null)
            {
                TempData["AlertMessage"] = "Oops! This Query Reason cannot be deleted!";
            }
            db.QueryReason.Remove(queryReason);
            TempData["success"] = "Query Reason Deleted Successfully";
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var userRetrieved = _uow.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
            var userName = fullName.ToString();
            await db.SaveChangesAsync(userName);
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "QueryReason cannot be deleted since it has an Order Query associated";
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
