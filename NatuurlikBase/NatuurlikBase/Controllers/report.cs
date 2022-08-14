using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using Newtonsoft.Json;
using NatuurlikBase.Models;
using NatuurlikBase.ViewModels;
using System.Globalization;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class report : Controller
    {

        private DatabaseContext db;
        public report(DatabaseContext _db)
        {
            db = _db;
        }
        public IActionResult Index()
        {
            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);

            var actorName = db.Users.Where(x => x.Id == claim.Value).FirstOrDefault();

            ViewBag.ActorName = actorName.FirstName;
            ViewBag.Surname = actorName.Surname;
            return View();
        }

       

        //User Sales Bar Graph againist orderTotal minus deliveryFees  
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
                double Samount = obj.Sum(o => o.Amount);
                list.Add(new { Amount = Math.Round(userOrder.Amount,2), Name = userOrder.Name + " " + userOrder.Surname, sales = Math.Round(Samount,2), Fname = userOrder.Name, lName = userOrder.Surname });
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
                decimal Samount = Math.Round((decimal)obj.Sum(o => o.Amount), 2);
                list.Add(new { Amount = Math.Round(monthlyOrder.Amount,2), Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthlyOrder.Month) + " " + monthlyOrder.Year, month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthlyOrder.Month), year = monthlyOrder.Year, sales = Math.Round(Samount,2) });
            }

            return JsonConvert.SerializeObject(list);


        }
        [HttpGet]
        //Product sales
        public string GetProductales()
        {



            List<Productsales> obj = db.OrderLine.Select(o => new Productsales
            {

                salesAmount = Math.Round((double)o.Price * o.Count,2),
                ProductAmount = Math.Round((double)o.Count),
                Product = o.ProductId


            }).ToList();

            List<object> list = new List<object>();
            foreach (Product product in db.Products.ToList())
            {
                double SALESamount = obj.Where(o => o.Product == product.Id).Sum(o => o.salesAmount);
                double PRODUCTamount = obj.Where(o => o.Product == product.Id).Sum(o => o.ProductAmount);
                double Pamount = obj.Sum(o => o.ProductAmount);
                double Samount = obj.Sum(o => o.salesAmount);


                list.Add(new { salesAmount = Math.Round(SALESamount,2), ProductAmount = Math.Round(PRODUCTamount,2), Name = product.Name, amount = Math.Round(Pamount,2), Samount = Math.Round(Samount,2), });
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
                list.Add(new { Amount = Math.Round(amount,2), Name = product.Name });
            }

            return JsonConvert.SerializeObject(list);
        }
       
        
    }


}
    
