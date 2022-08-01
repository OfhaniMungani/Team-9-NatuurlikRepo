using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;
using PhoneNumbers;
using System.Security.Claims;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NatuurlikBase.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly DatabaseContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork uow, DatabaseContext db,IWebHostEnvironment hostEnvironment, IEmailSender emailSender, IConfiguration configuration)
        {
            _uow = uow;
            _db = db;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public ActionResult GetQueryReasons(int queryReasonId)
        {
            return Json(_uow.QueryReason.GetAll(x => x.Id == queryReasonId).Select(x => new
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList());
        }


        public IActionResult Detail(int? orderId)
        {
            //Load all order details
            OrderVM orderVM = new OrderVM()
            {
                Order = _uow.Order.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser,Country,Province,City,Suburb,Courier"),
                OrderLine = _uow.OrderLine.GetAll(ol => ol.OrderId == orderId, includeProperties: "Product")
            };

            return View(orderVM);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveResellerOrder()
        {
            //Retrieve Order details from the db
            var orderRetrieved = _uow.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id);
            //Update order status to approved state and save changes to db.
            _uow.Order.UpdateOrderStatus(OrderVM.Order.Id, SR.ProcessingOrder);
            _uow.Save();

            var user = _db.User.Where(z => z.Id == orderRetrieved.ApplicationUserId).FirstOrDefault();
            string email = user.Email;
            string name = user.FirstName;
            string number = orderRetrieved.Id.ToString();
            string date = orderRetrieved.CreatedDate.ToString("M");
            string status = orderRetrieved.OrderPaymentStatus.ToString();
            var callbackUrl = Url.Action("Index", "Order", values: null, protocol: Request.Scheme);

            string wwwRootPath = _hostEnvironment.WebRootPath;
            var template = System.IO.File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\appOrderTemp.html"));
            template = template.Replace("[NAME]", name).Replace("[STATUS]", status)
                .Replace("[ID]", number).Replace("[DATE]", date).Replace("[URL]", callbackUrl);
            string message = template;

            _emailSender.SendEmailAsync(
            email,
            "Order Approved",
            message);

            TempData["Success"] = "Reseller Order has been approved successfully.";
            return RedirectToAction("Detail", "Order", new { orderId = OrderVM.Order.Id });
          
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            //Retrieve Order details from the db
            var orderRetrieved = _uow.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id);
            //Update order status to approved state and save changes to db.
            _uow.Order.UpdateOrderStatus(OrderVM.Order.Id, SR.OrderCancelled);
            _uow.Save();

            var user = _db.User.Where(z => z.Id == orderRetrieved.ApplicationUserId).FirstOrDefault();
            string email = user.Email;
            string name = user.FirstName;
            string number = orderRetrieved.Id.ToString();
            string date = orderRetrieved.CreatedDate.ToString("M");
            string status = orderRetrieved.OrderPaymentStatus.ToString();
            var callbackUrl = Url.Action("Index", "Order", values: null, protocol: Request.Scheme);

            string wwwRootPath = _hostEnvironment.WebRootPath;
            var template = System.IO.File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\canResOrderTemp.html"));
            template = template.Replace("[NAME]", name).Replace("[STATUS]", status)
                .Replace("[ID]", number).Replace("[DATE]", date).Replace("[URL]", callbackUrl);
            string message = template;

            _emailSender.SendEmailAsync(
            email,
            "Order Cancelled",
            message);

            TempData["Success"] = "Order has been cancelled successfully.";
            return RedirectToAction("Detail", "Order", new { orderId = OrderVM.Order.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessOrder()
        {
            //Retrieve Order details from the db
            var orderRetrieved = _uow.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id);
            //Update order status to approved state and save changes to db.
            _uow.Order.UpdateOrderStatus(OrderVM.Order.Id, SR.ProcessingOrder);
            _uow.Save();
            TempData["Success"] = "Order status updated successfully.";
            return RedirectToAction("Detail", "Order", new { orderId = OrderVM.Order.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectResellerOrder()
        {
            //Retrieve Order details from the db
            var orderRetrieved = _uow.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id);
            //Update order status to approved state and save changes to db.
            _uow.Order.UpdateOrderStatus(OrderVM.Order.Id, SR.OrderRejected);
            _uow.Save();

            var user = _db.User.Where(z => z.Id == orderRetrieved.ApplicationUserId).FirstOrDefault();
            string email = user.Email;
            string name = user.FirstName;
            string number = orderRetrieved.Id.ToString();
            string date = orderRetrieved.CreatedDate.ToString("M");
            string status = orderRetrieved.OrderPaymentStatus.ToString();
            var callbackUrl = Url.Action("Index", "Order", values: null, protocol: Request.Scheme);

            string wwwRootPath = _hostEnvironment.WebRootPath;
            var template = System.IO.File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\rejOrderTemp.html"));
            template = template.Replace("[NAME]", name).Replace("[STATUS]", status)
                .Replace("[ID]", number).Replace("[DATE]", date).Replace("[URL]", callbackUrl);
            string message = template;

            _emailSender.SendEmailAsync(
            email,
            "Order Rejected",
            message);

            TempData["Success"] = "Reseller Order has been rejected successfully.";
            return RedirectToAction("Detail", "Order", new { orderId = OrderVM.Order.Id });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureResellerPayment()
        {
            //Retrieve Order details from the db
            var orderRetrieved = _uow.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id);
            //Update order status to approved state and save changes to db.
            _uow.Order.UpdateOrderPaymentStatus(OrderVM.Order.Id, SR.OrderPaymentApproved);
            _uow.Save();

            var user = _db.User.Where(z => z.Id == orderRetrieved.ApplicationUserId).FirstOrDefault();
            string email = user.Email;
            string name = user.FirstName;
            string number = orderRetrieved.Id.ToString();
            string date = DateTime.Now.ToString("M");
            string status = orderRetrieved.OrderPaymentStatus.ToString();
            string total = orderRetrieved.OrderTotal.ToString();
            var callbackUrl = Url.Action("Index", "Order", values: null, protocol: Request.Scheme);

            string wwwRootPath = _hostEnvironment.WebRootPath;
            var template = System.IO.File.ReadAllText(Path.Combine(wwwRootPath, @"emailTemp\paidTemp.html"));
            template = template.Replace("[NAME]", name).Replace("[STATUS]", status)
                .Replace("[ID]", number).Replace("[DATE]", date).Replace("[TOTAL]", total).Replace("[URL]", callbackUrl);
            string message = template;

            _emailSender.SendEmailAsync(
            email,
            "Payment Received",
            message);

            TempData["Success"] = "Reseller Order payment captured successfully.";
            return RedirectToAction("Detail", "Order", new { orderId = OrderVM.Order.Id });

        }

        public IActionResult DispatchParcel()
        {
            //Retrieve Order details from the db
            var orderRetrieved = _uow.Order.GetFirstOrDefault(u => u.Id == OrderVM.Order.Id);
            //Capture required details in order to ship/dispatch parcel.
            orderRetrieved.ParcelTrackingNumber = OrderVM.Order.ParcelTrackingNumber;
            orderRetrieved.DispatchedDate = DateTime.Now;
            orderRetrieved.OrderStatus = SR.OrderDispatched;
            _uow.Order.Update(orderRetrieved);
            _uow.Save();

            string accountId = _configuration["AccountId"];
            string authToken = _configuration["AuthToken"];
            TwilioClient.Init(accountId, authToken);
            var number = orderRetrieved.PhoneNumber;
            var name = orderRetrieved.FirstName;
            var order = orderRetrieved.Id;
            var track = orderRetrieved.ParcelTrackingNumber;
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var numberProto = phoneUtil.Parse(number, "ZA");
            var formattedPhone = phoneUtil.Format(numberProto, PhoneNumberFormat.E164);
            var to = formattedPhone;
            var companyNr = "+18305216564";

            var message = MessageResource.Create(
                to,
                from: companyNr,
                body: $"Hi " + name + " your Natuurlik order #" + order + " has been dispatched, your tracking number is " + track);

            TempData["Success"] = "Order status updated successfully.";
            return RedirectToAction("Detail", "Order", new { orderId = OrderVM.Order.Id });

        }
        public IActionResult ViewQueries()

        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderQuery> orderQueries;

            if (User.IsInRole(SR.Role_Admin) || User.IsInRole(SR.Role_SA))
            {
                orderQueries = _uow.OrderQuery.GetAll(includeProperties: "Order,QueryReason");
            }
            //Allow only for order to be queried which user placed
            else
            {
                orderQueries = _uow.OrderQuery.GetAll(x => x.Order.ApplicationUserId == claim.Value, includeProperties: "Order,QueryReason");
            }

            return View(orderQueries);
        }

        public IActionResult ViewReviews()

        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderReview> orderReview;

            if (User.IsInRole(SR.Role_Admin) || User.IsInRole(SR.Role_SA))
            {
                orderReview = _uow.OrderReview.GetAll(includeProperties: "order,ReviewReason");
            }
            //Allow only for order to be queried which user placed
            else
            {
                orderReview = _uow.OrderReview.GetAll(x => x.order.ApplicationUserId == claim.Value, includeProperties: "order,ReviewReason");
            }

            return View(orderReview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateQuery(OrderQueryVM orderQueryVM)
        {
            orderQueryVM.OrderQuery.QueryStatus = SR.QueryLogged;
            _db.OrderQuery.Add(orderQueryVM.OrderQuery);
            _db.SaveChanges();
            TempData["success"] = "Your query has been submitted successfully.";
            return RedirectToAction("ViewQueries");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateReview(OrderReviewVMcs orderReviewVM)
        {
            //orderReviewVM.Order.QueryStatus = SR.QueryLogged;
            _db.OrderReview.Add(orderReviewVM.OrderReview);
            _db.SaveChanges();
            TempData["success"] = "Your Review has been submitted successfully.";
            return RedirectToAction("Index");

        }
        //GET
        public IActionResult ReviewQuery(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var orderQuery = _uow.OrderQuery.GetFirstOrDefault(u => u.Id == id, includeProperties: "Order,QueryReason");


            if (orderQuery == null)
            {
                return NotFound();
            }

            return View(orderQuery);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ReviewQuery(OrderQuery orderQuery)

        {
            orderQuery.QueryStatus = SR.QueryReview;
            orderQuery.QueryFeedback = orderQuery.QueryFeedback;
            _db.OrderQuery.Update(orderQuery);
            _db.SaveChanges();
            TempData["success"] = "The order query was reviewed successfully.";
            return RedirectToAction("ViewQueries");

        }


        [HttpGet]
        public IActionResult LogQuery(int orderId)
        {
            if (User.IsInRole(SR.Role_Admin) || User.IsInRole(SR.Role_SA))
            {
                OrderQueryVM orderQueryVM = new()
                {
                    OrderQuery = new(),
                    OrdersList = _uow.Order.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Id.ToString(),
                        Value = i.Id.ToString()
                    }),
                    QueryReasons = _uow.QueryReason.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    })
                };

                _db.SaveChanges();

                return View(orderQueryVM);
            }
            else
            {
                //get the orders associated only with the customer or reseller.


                OrderQueryVM orderQueryVM = new()
                {
                    OrderQuery = new(),
                    OrdersList = _db.Order.Where(x => x.Id == orderId).Select(i => new SelectListItem
                    {
                        Text = i.Id.ToString(),
                        Value = i.Id.ToString()
                    }),
                    QueryReasons = _uow.QueryReason.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    })
                };
                orderQueryVM.OrderQuery.OrderId = orderId;
                ViewBag.Confirmation = "Confirm order query details?";
                _db.SaveChanges();

                return View(orderQueryVM);
            }

        }

        [HttpGet]
        public IActionResult LogReview(int orderId)
        {
            if (User.IsInRole(SR.Role_Admin) || User.IsInRole(SR.Role_SA))
            {
                OrderReviewVMcs orderReviewVM = new()
                {
                    OrderReview = new(),
                    OrdersList = _uow.Order.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Id.ToString(),
                        Value = i.Id.ToString()
                    }),
                    ReviewReasons = _uow.ReviewReason.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    })
                };

                _db.SaveChanges();

                return View(orderReviewVM);
            }
            else
            {
                //get the orders associated only with the customer or reseller.


                OrderReviewVMcs orderReviewVM = new()
                {
                    OrderReview = new(),
                    OrdersList = _db.Order.Where(x => x.Id == orderId).Select(i => new SelectListItem
                    {
                        Text = i.Id.ToString(),
                        Value = i.Id.ToString()
                    }),
                    ReviewReasons = _uow.ReviewReason.GetAll().Select(i => new SelectListItem
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    })
                };
                orderReviewVM.OrderReview.OrderId = orderId;
                ViewBag.Confirmation = "Confirm order query details?";
                _db.SaveChanges();

                return View(orderReviewVM);
            }

        }
        public IActionResult QueryDetail(int? queryId)
        {
            //Load all order details
            OrderQuery orderquery = _uow.OrderQuery.GetFirstOrDefault(x => x.Id == queryId, includeProperties: "Order,QueryReason");

            return View(orderquery);
        }

        public IActionResult Index(string status)
        {
            IEnumerable<Order> orders;

            if (User.IsInRole(SR.Role_Admin) || User.IsInRole(SR.Role_SA))
            {
                //Retrieve all orders for Administrator and Sales Assistant roles.
                 orders = _uow.Order.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                //get the orders associated only with the customer or reseller.
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orders = _uow.Order.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }
           

            switch(status)
            {
                case "processing":
                    orders = orders.Where(u => u.OrderStatus == SR.ProcessingOrder);
                    break;

                case "pending":
                    orders = orders.Where(o => o.OrderStatus == SR.OrderPending);
                    break;

                case "dispatched":
                    orders = orders.Where(o => o.OrderStatus == SR.OrderDispatched);
                    break;

                case "cancelled":
                    orders = orders.Where(o => o.OrderStatus == SR.OrderCancelled);
                    break;

                case "refundpending":
                    orders = orders.Where(o => o.OrderStatus == SR.OrderRefundPending);
                    break;

                case "refunded":
                    orders = orders.Where(o => o.OrderStatus == SR.OrderRefunded);
                    break;

                default:
                    break;
            }
            return View(orders);
        }

       

    }
}
