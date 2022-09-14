using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Models.ViewModels;
using NatuurlikBase.Repository.IRepository;
using System.Security.Claims;

namespace NatuurlikBase.Controllers;
//[Authorize(Roles = SR.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly DatabaseContext _context;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, DatabaseContext context)
    {
        _unitOfWork = unitOfWork;
        _hostEnvironment = hostEnvironment;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    //GET
    public IActionResult Upsert(int? id)
    {
        ProductVM productVM = new()
        {
            Product = new(),
            CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
            CoverTypeList = _unitOfWork.Brand.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }),
        };

        if (id == null || id == 0)
        {
           
            return View(productVM);
        }
        else
        {
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);

        }


    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductVM obj, IFormFile? file)
    {

        if (ModelState.IsValid)
        {

            if (_context.Products.Any(c => c.Name==obj.Product.Name && c.CustomerPrice==obj.Product.CustomerPrice && c.ResellerPrice == obj.Product.ResellerPrice 
            && c.Description == obj.Product.Description && c.CategoryId == obj.Product.CategoryId && c.ProductBrandId == obj.Product.ProductBrandId && 
            c.PictureUrl == obj.Product.PictureUrl && c.ThresholdValue == obj.Product.ThresholdValue && c.DisplayProduct == obj.Product.DisplayProduct))
       
               
            {

                ViewBag.Error = "Product Already Exists!";
                                

            }
                else 
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Product.PictureUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.PictureUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.PictureUrl = @"\images\products\" + fileName + extension;

                }
                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                    TempData["success"] = "Product created successfully";
                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                    var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                    var userName = fullName.ToString();
                    await _context.SaveChangesAsync(userName);

                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
                    var claimsId = (ClaimsIdentity)User.Identity;
                    var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
                    var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
                    var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
                    var userName = fullName.ToString();
                    await _context.SaveChangesAsync(userName);
                }
                _unitOfWork.Save();
                
                return RedirectToAction("Index");
            }
        }
        return View(obj);
    }



   #region API CALLS
   [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,Brand");
        return Json(new { data = productList });
    }


    public async Task<IActionResult> Delete(int? id)
    {
       

        var ForeignKey = _unitOfWork.OrderLine.GetAll().Any(x => x.ProductId == id);
        if (ForeignKey)
        {
            TempData["Delete"] = "Product cannot be deleted since it has an association with order";
            
             return Json(new { success = false, message = "Product cannot be deleted since it has an association with order" });
        }
        else
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.PictureUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var userRetrieved = _unitOfWork.User.GetFirstOrDefault(x => x.Id == claim.Value);
            var fullName = userRetrieved.FirstName + " " + userRetrieved.Surname;
            var userName = fullName.ToString();
            await _context.SaveChangesAsync(userName);
            TempData["successDelete"] = "Product deleted successfully";
            return Json(new { success = true, message = "Delete Successful" });
        }
       
    }
    #endregion
}
