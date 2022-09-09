using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Controllers;
[Authorize(Roles = SR.Role_Admin + "," + SR.Role_SA)]
public class ReturnedProductController : Controller
{
    private readonly DatabaseContext db;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IUnitOfWork _unitOfWork;


    public ReturnedProductController(DatabaseContext context, IWebHostEnvironment hostEnvironment, IUnitOfWork unitOfWork)
    {
        db = context;
        _hostEnvironment = hostEnvironment;
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        var databaseContext = db.ReturnedProduct.Include(s => s.ReturnReason).Include(p => p.Product).Include(o => o.Order);
        return View(databaseContext.ToList());
    }

    public async Task<IActionResult> Create(int orderId)
    {
        ReturnedProduct returnedProduct = new ReturnedProduct();

        returnedProduct.OrderId = orderId;

        var returned = db.Products.Where(c => c.Id == returnedProduct.ProductId).FirstOrDefault();
        ViewData["OrderId"] = new SelectList(db.Order, "Id", "Id");
        ViewData["ProductId"] = new SelectList(db.Products, "Id", "Name");
        //ViewData["OrderLineId"] = new SelectList(db.OrderLine, "Id", "ProductId", "Count");
        ViewData["ReturnReasonId"] = new SelectList(db.ReturnReason, "Id", "ReturnReasonName");
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([Bind("Id,QuantityReceived,DateLogged,OrderId,ProductId,ReturnReasonId")] ReturnedProduct returnedProduct)
    {
        if (ModelState.IsValid)

        {
            var inv = db.Products.Where(s => s.Id == returnedProduct.ProductId).FirstOrDefault();
            if (inv != null)
            {
                inv.QuantityOnHand += returnedProduct.QuantityReceived;
            }

            var orderRetrieved = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == returnedProduct.OrderId);
            //Update order status to approved state and save changes to db.
            _unitOfWork.Order.UpdateOrderStatus(returnedProduct.OrderId, SR.ReturnedProduct);

            var order = db.Order.Where(c => c.Id == returnedProduct.OrderId).FirstOrDefault();
            var orderLine = db.OrderLine.Where(x => x.OrderId == order.Id).Where(o => o.ProductId == returnedProduct.ProductId).FirstOrDefault();

            var returnedCount = db.OrderProduct.Where(x => x.OrderId == order.Id).Where(p => p.ProductId == returnedProduct.ProductId).FirstOrDefault();

            if (orderLine == null)
            {
                TempData["Error"] = "Oops! You selected the incorrect product!";
                return RedirectToAction("Index");

            }
            else

            if (returnedCount == null)
            {
                if (orderLine.Count < returnedProduct.QuantityReceived)
                {
                    TempData["CountError"] = "Oops! You attempted to return more than what was ordered!";
                    return RedirectToAction("Index");
                }
            }


            db.ReturnedProduct.Add(returnedProduct);
            ViewBag.ReturnedProductConfirmation = "Confirm Returned Product Details";
            db.SaveChanges();

            TempData["success"] = "Returned Product Captured Successfully!";

            return RedirectToAction("Index");
        }
        ViewData["OrderId"] = new SelectList(db.Order, "Id", returnedProduct.OrderId.ToString());
        ViewData["ProductId"] = new SelectList(db.Products, "Id", "Name", returnedProduct.ProductId.ToString());
        //ViewData["OrderLineId"] = new SelectList(db.OrderLine, "Id", "ProductId", "Count", returnedProduct.OrderLineId.ToString());
        ViewData["ReturnReasonId"] = new SelectList(db.ReturnReason, "Id", "ReturnReasonName", returnedProduct.ReturnReasonId.ToString());
        return View(returnedProduct);
    }

}
