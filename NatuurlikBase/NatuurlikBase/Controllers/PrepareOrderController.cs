﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Models.ViewModels;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class PrepareOrderController : Controller
    {
        private readonly DatabaseContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IUnitOfWork _unitOfWork;


        public PrepareOrderController(DatabaseContext db, IEmailSender emailSender, IWebHostEnvironment hostEnvironment, IUnitOfWork unitOfWork)
        {
            _db = db;
            _emailSender = emailSender;
            _hostEnvironment = hostEnvironment;
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index(int orderId)
        {
            PrepareOrderVM model = new PrepareOrderVM();
            model.PackageOrderProduct = _db.OrderProduct.Where(x => x.OrderId == orderId).Include(c => c.Order).Include(b => b.Product);
            model.Order = _db.Order.FirstOrDefault(x => x.Id == orderId);
            return View(model);
        }

        public ActionResult GetOrders(int orderId)
        {
            return Json(_db.Order.Where(x => x.Id == orderId).Select(x => new
            {
                Text = x.Id,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList());
        }


        public ActionResult GetProducts(int prodId)
        {
            return Json(_db.Products.Where(x => x.Id == prodId).Select(x => new
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList());
        }


        public IActionResult Create()
        {

            ViewData["OrderId"] = new SelectList(_db.Order, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_db.Products, "Id", "Name");
            return View();
        }

        [HttpGet]
        public IActionResult Create(int orderId)
        {

            //Get all products associated with the order
            //var prodId = _db.OrderProduct.Where(x => x.OrderId == orderId).Select(x => x.ProductId).ToList();


            PackageOrderVM packageOrderVM = new PackageOrderVM()
            {
                PackageOrderProduct = new(),
                ProductList = _unitOfWork.Product.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };

            //populate the requested order

            packageOrderVM.PackageOrderProduct.OrderId = orderId;

            var prod = _db.Products.Where(c => c.Id == packageOrderVM.PackageOrderProduct.ProductId).FirstOrDefault();

            //if(ModelState.IsValid)
            //{
            //    if(prod != null)
            //    {
            //        if (prod.QuantityOnHand > packageOrderVM.PackageOrderProduct.ProductQuantity || prod.QuantityOnHand == packageOrderVM.PackageOrderProduct.ProductQuantity)

            //        {
            //            prod.QuantityOnHand -= packageOrderVM.PackageOrderProduct.ProductQuantity;
            //            //_db.OrderProduct.Add(packageOrderVM.PackageOrderProduct);
            //            //ViewBag.Confirmation = "Are you sure you want to proceed with removal?";
            //            _db.SaveChangesAsync();
            //            TempData["success"] = "Product Packaged Successfully.";


            //            return RedirectToAction(nameof(Index));
            //        }
            //        else
            //        {
            //            ViewBag.Error = "Insufficient stock levels to package requested quantity.";
            //        }
            //    }

            //}


            return View(packageOrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("PrepareOrder")]
        public IActionResult Capture(PackageOrderVM packageOrderVM)
        {
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);

            var actorName = _db.Users.Where(x => x.Id == claim.Value).FirstOrDefault();

            packageOrderVM.PackageOrderProduct.ActorName = actorName.FirstName;
            packageOrderVM.PackageOrderProduct.OrderId = packageOrderVM.PackageOrderProduct.OrderId;
            //check to not package more than what is available on hand.
            var prod = _db.Products.Where(c => c.Id == packageOrderVM.PackageOrderProduct.ProductId).FirstOrDefault();
            var order = _db.Order.Where(c => c.Id == packageOrderVM.PackageOrderProduct.OrderId).FirstOrDefault();

            if (order.IsResellerOrder == true)
            {

                if (prod != null)
                {
                    if (prod.QuantityOnHand > packageOrderVM.PackageOrderProduct.ProductQuantity || prod.QuantityOnHand == packageOrderVM.PackageOrderProduct.ProductQuantity)

                    {
                        prod.QuantityOnHand -= packageOrderVM.PackageOrderProduct.ProductQuantity;
                        TempData["success"] = "Product Packaged Successfully.";
                        _db.OrderProduct.Add(packageOrderVM.PackageOrderProduct);
                        _db.SaveChanges();
                        return Redirect("PrepareOrder?orderId=" + packageOrderVM.PackageOrderProduct.OrderId.ToString());

                    }
                    else
                    {
                        TempData["error"] = "Insufficient stock available to package the requested quantity.";
                        return Redirect("PrepareOrder?orderId=" + packageOrderVM.PackageOrderProduct.OrderId.ToString());
                    }
                }
            }
            else
            {
                TempData["success"] = "Product Packaged Successfully.";
                _db.OrderProduct.Add(packageOrderVM.PackageOrderProduct);
                _db.SaveChanges();
                return Redirect("PrepareOrder?orderId=" + packageOrderVM.PackageOrderProduct.OrderId.ToString());
            }
            return RedirectToAction("Create");
        }
    }
}
