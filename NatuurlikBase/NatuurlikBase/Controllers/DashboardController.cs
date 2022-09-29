using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.ViewModels;
using Newtonsoft.Json;
using System.Globalization;

namespace NatuurlikBase.Controllers
{
    public class DashboardController : Controller
    {
        private DatabaseContext db;
        public DashboardController(DatabaseContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            var sales = db.Order.Sum(p => p.OrderTotal - p.DeliveryFee);
            ViewBag.sales = sales;

            IEnumerable<ApplicationUser> users = (from user in db.Users
                                                  join userRole in db.UserRoles on user.Id equals userRole.UserId
                                                  join role in db.Roles on userRole.RoleId equals role.Id
                                                  where role.Name == "Customer" && user.EmailConfirmed == true
                                                  select user)
                                                    .ToList();
            ViewBag.users = users.Count();


            IEnumerable<ApplicationUser> res = (from user in db.Users
                                                join userRole in db.UserRoles on user.Id equals userRole.UserId
                                                join role in db.Roles on userRole.RoleId equals role.Id
                                                where role.Name == "Reseller" && user.EmailConfirmed == true
                                                select user)
                                                    .ToList();
            ViewBag.res = res.Count();

            var owing = db.Order.Count(c => c.OrderPaymentStatus == SR.PaymentOverdue );
            ViewBag.owing = owing;

            var due = db.Order.Count(c => c.OrderPaymentStatus == SR.ResellerDelayedPayment);
            ViewBag.due = due;

            return View();
        }


        public IActionResult PowerBI()
        {
            return View();
        }

        public string GetOrderData()
        {

            List<object> list = new List<object>();
            List<userOrder> obj = db.Order.GroupBy(i => new
            {
                UserId = i.ApplicationUserId,
                Name = i.FirstName,
                Surname = i.Surname
            })
            .Select(o => new userOrder
            {
                UserID = o.Key.UserId,
                Name = o.Key.Name,
                Surname = o.Key.Surname,
                Amount = Math.Round((double)o.Sum(p => p.OrderTotal - p.DeliveryFee), 2),

            }).Take(5).ToList();

            foreach (var userOrder in obj)
            {
                //double Pamount = obj.Sum(o => o.ProductAmount);
                string Samount = obj.Sum(o => o.Amount).ToString("C", CultureInfo.CurrentCulture);
                TempData["monthTotal"] = Samount;
                list.Add(new { tableAmount = userOrder.Amount.ToString("C", CultureInfo.CurrentCulture), Amount = Math.Round(userOrder.Amount, 2), Name = userOrder.Name + " " + userOrder.Surname, sales = Samount, Fname = userOrder.Name, lName = userOrder.Surname });
            }

            return JsonConvert.SerializeObject(list);


        }
        //Monthly Sales Line Graph againist orderTotal minus deliveryFees  
        public string GetOrderDataByDate()
        {


            List<object> list = new List<object>();
            List<MonthlyOrder> obj = db.Order.GroupBy(i => new
            {
                Month = i.CreatedDate.Month,
                Year = i.CreatedDate.Year
            })
                .Select(o => new MonthlyOrder
                {

                    Amount = Math.Round((double)o.Sum(p => p.OrderTotal - p.DeliveryFee), 2),
                    Month = o.Key.Month,
                    Year = o.Key.Year


                }).ToList();

            //render chart data
            foreach (var monthlyOrder in obj)
            {
                string Samount = obj.Sum(o => o.Amount).ToString("C", CultureInfo.CurrentCulture); //total          
                list.Add(new { tableAmount = monthlyOrder.Amount.ToString("C", CultureInfo.CurrentCulture), Amount = Math.Round(monthlyOrder.Amount, 2), Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthlyOrder.Month) + " " + monthlyOrder.Year, month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthlyOrder.Month), year = monthlyOrder.Year, sales = Samount });
            }

            return JsonConvert.SerializeObject(list);


        }
        [HttpGet]
        //Product sales
        public string GetProductales()
        {
            List<Productsales> obj = db.OrderLine.Select(o => new Productsales
            {

                salesAmount = Math.Round((double)o.Price * o.Count, 2),
                ProductAmount = Math.Round((double)o.Count),
                Product = o.ProductId


            }).ToList();

            List<object> list = new List<object>();
            foreach (Product product in db.Products.ToList())
            {
                double SALESamount = obj.Where(o => o.Product == product.Id).Sum(o => o.salesAmount);
                double PRODUCTamount = obj.Where(o => o.Product == product.Id).Sum(o => o.ProductAmount);
                double Pamount = obj.Sum(o => o.ProductAmount);
                string Samount = obj.Sum(o => o.salesAmount).ToString("C", CultureInfo.CurrentCulture); //TOTAL


                list.Add(new { tableAmount = SALESamount.ToString("C", CultureInfo.CurrentCulture), salesAmount = Math.Round(SALESamount, 2), ProductAmount = Math.Round(PRODUCTamount, 2), Name = product.Name, amount = Math.Round(Pamount, 2), Samount = Samount });
            }

            return JsonConvert.SerializeObject(list);



        }
        //pie chart table
        public string GetProductOrderData()
        {
            List<ProductOrder> obj = db.OrderLine.Select(o => new ProductOrder
            {
                OrderID = o.ProductId,
                Amount = (double)o.Price * o.Count,

            }).ToList();

            List<object> list = new List<object>();
            foreach (Product product in db.Products.ToList())
            {
                double amount = obj.Where(o => o.OrderID == product.Id).Sum(o => o.Amount);
                list.Add(new { Amount = Math.Round(amount, 2), Name = product.Name });
            }

            return JsonConvert.SerializeObject(list);
        }

    }
}
