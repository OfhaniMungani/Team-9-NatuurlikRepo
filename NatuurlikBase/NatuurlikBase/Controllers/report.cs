using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using Newtonsoft.Json;
using NatuurlikBase.Models;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.ViewModels;

namespace NatuurlikBase.Controllers
{
    public class report : Controller
    {

        private DatabaseContext db;

        public report(DatabaseContext _db)
        {
            db = _db;
        }

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }
        public string GetOrderData()
        {
            List<userOrder> obj = db.OrderLine.Select(o => new userOrder
            {
                OrderID = o.OrderId,
                Amount = (double)o.Price * o.Count,
                UserID = o.Order.ApplicationUserId,
            }).ToList();

            List<object> list = new List<object>();
            foreach (ApplicationUser user in db.User.ToList())
            {
                double amount = obj.Where(o => o.UserID == user.Id).Sum(o => o.Amount);
                list.Add(new { Amount = Math.Round(amount), Name = user.FirstName + " " + user.Surname });
            }

            return JsonConvert.SerializeObject(list);
        }
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
                list.Add(new { Amount = Math.Round(amount), Name = product.Name});
            }

            return JsonConvert.SerializeObject(list);
        }
    }


}
    
