using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;

//[Authorize(Roles = SR.Role_Admin)]
public class ProductCategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DatabaseContext _db;

    public ProductCategoryController(IUnitOfWork unitOfWork, DatabaseContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public IActionResult Index()
    {
        IEnumerable<ProductCategory> objCategoryList = _unitOfWork.Category.GetAll();
        return View(objCategoryList);
    }

    //GET
    public IActionResult Create()
    {
        return View();
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCategory obj)
    {
        if (ModelState.IsValid)
        {
            if (_db.Categories.Any(c => c.Name.Equals(obj.Name)))
            {
                ViewBag.Error = "Product Category Already Exists!";

            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                _unitOfWork.Category.Add(obj);
                await _db.SaveChangesAsync(userName);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully";
                TempData["NextCreation"] = "Hello World.";
                return RedirectToAction("Index");
            }
        }
        return View(obj);
    }

    //GET
    public async Task<IActionResult> Edit(int? id)
    {

        if (id == null || id == 0)
        {
            return NotFound();
        }

        var productCategory = await _db.Categories.FindAsync(id);


        if (id == null)
        {
            return NotFound();
        }

        return View(productCategory);
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProductCategory obj)
    {

        if (ModelState.IsValid)
        {
            if (_db.Categories.Any(c => c.Name.Equals(obj.Name)))
            {
                ViewBag.Error = "Product Category Already Exists!";

            }
            else
            {
                var claimsId = (ClaimsIdentity)User.Identity;
                var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                var fullName = user.FirstName + " " + user.Surname;
                var userName = fullName.ToString();

                _unitOfWork.Category.Update(obj);
                await _db.SaveChangesAsync(userName);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
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

        var categoryFromDbFirst = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);

        if (categoryFromDbFirst == null)
        {
            return NotFound();
        }

        return View(categoryFromDbFirst);
    }

    //POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePOST(int? id)
    {
        
        ViewBag.CountryConfirmation = "Are you sure you want to delete this Product Category?";

        var hasFk = _unitOfWork.Product.GetAll().Any(x => x.CategoryId == id);

        if (!hasFk)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
            if (category == null)
            {
                TempData["AlertMessage"] = "Error occurred while attempting delete";
            }
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = user.FirstName + " " + user.Surname;
            var userName = fullName.ToString();

            _unitOfWork.Category.Remove(category);
            await _db.SaveChangesAsync(userName);
            _unitOfWork.Save();
            TempData["success"] = "Product Category Successfully Deleted.";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["Delete"] = "Product Category cannot be deleted since it is associated to a product";
            return RedirectToAction("Index");
        }





        //var obj = _unitOfWork.Category.GetFirstOrDefault(u => u.Id == id);
        //if (obj == null)
        //{
        //    return NotFound();
        //}

        //_unitOfWork.Category.Remove(obj);
        //ViewBag.Confirmation = "Are You Sure You Want To Delete This Category?.";
        //_unitOfWork.Save();
        //TempData["success"] = "Category deleted successfully";
        //return RedirectToAction("Index");

    }
}
