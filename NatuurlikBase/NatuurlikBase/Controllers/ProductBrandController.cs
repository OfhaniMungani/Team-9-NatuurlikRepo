using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;
[Authorize(Roles = SR.Role_Admin)]
public class ProductBrandController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DatabaseContext _db;
    public ProductBrandController(IUnitOfWork unitOfWork, DatabaseContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public IActionResult Index()
    {
        IEnumerable<ProductBrand> objCoverTypeList = _unitOfWork.Brand.GetAll();
        return View(objCoverTypeList);
    }

    //GET
    public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductBrand obj)
    {
        if (ModelState.IsValid)
        {

            if (_db.Brands.Any(c => c.Name.Equals(obj.Name)))
            {
                ViewBag.Error = "Product Brand Name Already Exist In The Database.";

            }
            else { 
                _unitOfWork.Brand.Add(obj);
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await _db.SaveChangesAsync(userName);
                TempData["success"] = "Product Brand created successfully";
                TempData["NextCreation"] = "Hello World.";
                return RedirectToAction("Index");
                 }
        }
        return View(obj);
    }

    //GET
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var CoverTypeFromDbFirst = _unitOfWork.Brand.GetFirstOrDefault(u => u.Id == id);

        if (CoverTypeFromDbFirst == null)
        {
            return NotFound();
        }

        return View(CoverTypeFromDbFirst);
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductBrand obj)
    {

        if (ModelState.IsValid)
        {

            if (_db.Brands.Any(c => c.Name.Equals(obj.Name)))
            {
                ViewBag.Error = "Product Brand Name Already Exist In The Database.";

            }
            else { 
            _unitOfWork.Brand.Update(obj);
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                var userName = fullName.ToString();
                await _db.SaveChangesAsync(userName);
                TempData["success"] = "Product Brand Updated Successfully";
            return RedirectToAction("Index");
            }
        }
        return View(obj);
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var CoverTypeFromDbFirst = _unitOfWork.Brand.GetFirstOrDefault(u => u.Id == id);

        if (CoverTypeFromDbFirst == null)
        {
            return NotFound();
        }

        return View(CoverTypeFromDbFirst);
    }

    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePOST(int? id)
    {
        ViewBag.CountryConfirmation = "Are you sure you want to delete this Product Brand?";

        var hasFk = _unitOfWork.Product.GetAll().Any(x => x.ProductBrandId == id);

        if (!hasFk)
        {
            var brand = _unitOfWork.Brand.GetFirstOrDefault(u => u.Id == id);
            if (brand == null)
            {
                TempData["AlertMessage"] = "Error occurred while attempting delete";
            }
            _unitOfWork.Brand.Remove(brand);
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
            var userName = fullName.ToString();
            await _db.SaveChangesAsync(userName);
            TempData["success"] = "Product Brand Successfully Deleted.";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "Product Brand cannot be deleted since it is associated to a product";
            return RedirectToAction("Index");
        }
    }
}
