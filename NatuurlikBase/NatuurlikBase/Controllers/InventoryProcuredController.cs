using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;
[Authorize(Roles = SR.Role_Admin)]
public class InventoryProcuredController : Controller
{
    private readonly DatabaseContext db;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IUnitOfWork _unitOfWork;

    public InventoryProcuredController(DatabaseContext context, IWebHostEnvironment hostEnvironment, IUnitOfWork unitOfWork)
    {
        db = context;
        _hostEnvironment = hostEnvironment;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var databaseContext = db.InventoryProcured.Include(c => c.Supplier).Include(x => x.InventoryItem);
        return View(databaseContext.ToList());
    }

    public IActionResult Create()
    {
        ViewData["SupplierId"] = new SelectList(db.Suppliers, "Id", "CompanyName");
        ViewData["ItemID"] = new SelectList(db.InventoryItem, "Id", "InventoryItemName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,SupplierId,InvoiceNo,ItemID,QuantityReceived,DateLogged,InvoiceFile")] InventoryProcured inventoryProcured, IFormFile? file)
    {
        if (ModelState.IsValid)

        {
            var inv = db.InventoryItem.Where(s => s.Id == inventoryProcured.ItemID).FirstOrDefault();
            if (inv != null)
            {
                inv.QuantityOnHand += inventoryProcured.QuantityReceived;
            }

            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"invoices\");
                var extension = Path.GetExtension(file.FileName);

                if (extension != ".pdf" && extension != ".png" && extension != ".jpeg" && extension != ".jpg") //CHECK IMAGE EXTENSION CODE
                {
                    TempData["wrong"] = "Please upload a pdf, jpeg, jpg or png";
                }

                else
                {
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    inventoryProcured.InvoiceFile = @"\invoices\" + fileName + extension;

                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                    var fullName = user.FirstName + " " + user.Surname;
                    var userName = fullName.ToString();

                    ViewBag.InventoryProcuredConfrimation = "Confirm Procured Inventory Details";
                    db.InventoryProcured.Add(inventoryProcured);
                    await db.SaveChangesAsync(userName);

                    TempData["success"] = "Procured Inventory Captured Successfully!";
                    return RedirectToAction("Index");

                }
            }
        }
        ViewData["SupplierId"] = new SelectList(db.Suppliers, "Id", "CompanyName", inventoryProcured.SupplierId);
        ViewData["ItemID"] = new SelectList(db.InventoryItem, "Id", "InventoryItemName", inventoryProcured.ItemID);
        return View(inventoryProcured);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var inventoryProcured = await db.InventoryProcured.Include(c => c.Supplier).
            Include(x => x.InventoryItem).FirstOrDefaultAsync(m => m.Id == id);
        if (inventoryProcured == null)
        {
            return NotFound();
        }

        return View(inventoryProcured);
    }


}

