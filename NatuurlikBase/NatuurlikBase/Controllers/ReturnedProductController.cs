using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Controllers
{
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

        public async Task<IActionResult> Create(int? id)
        {
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



                //if (returnedProduct.QuantityReceived > orderLine.Count)
                //{
                //    ViewBag.CountError = "Quantity Logged is more than the quantity ordered, Please re-enter the details";
                //}
                var orderRetrieved = _unitOfWork.Order.GetFirstOrDefault(u => u.Id == returnedProduct.OrderId);
                //Update order status to approved state and save changes to db.
                _unitOfWork.Order.UpdateOrderStatus(returnedProduct.OrderId, SR.ReturnedProduct);


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
}
