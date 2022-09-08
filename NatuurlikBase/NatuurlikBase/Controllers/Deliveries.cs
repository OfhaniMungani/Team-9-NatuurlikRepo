using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;

namespace NatuurlikBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class Deliveries : Controller
    {
        private readonly DatabaseContext  db ;
        private readonly IUnitOfWork _uow;
       
        public Deliveries(DatabaseContext _db, IUnitOfWork uow)
        {
            db = _db;
            _uow = uow;
        }

        // Creating a order list. The list will contain all orders.
        [HttpGet]
        [Route("getDelivery")]
        public async Task<ActionResult<dynamic>> GetOrders()
        {
            try
            {
                List<dynamic> deliverylist = new List<dynamic>();



                dynamic orders = db.Order.Where(o=>o.Suburb.SuburbName== "Garsfontein").Where(d=>d.OrderStatus==SR.OrderDispatched).Select(o => new
                {
                    id = o.Id,
                    FirstName = o.FirstName,
                    Surname = o.Surname,
                    StreetAddress = o.StreetAddress,
                    City = o.City.CityName,
                    Country = o.Country.CountryName,
                    Province = o.Province.ProvinceName,
                    Suburb = o.Suburb.SuburbName,
                    date = o.CreatedDate,
                    phoneNumber = o.PhoneNumber
                });

                deliverylist.Add(orders);

                return deliverylist;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }
        }
        [HttpPost]
        [Route("createDelivery")]
        public object CreateDelivery(DeliveriesVM VM)
        {
         

            try
            {
                
                foreach (var item in VM.DeliveriesV)
                {
                   
                   
                      OrderVM orderVM = new OrderVM()
                    {
                    Order = _uow.Order.GetFirstOrDefault(u => u.Id ==item.id),

                     };
                    Delivery deliveryCreate = new Delivery
                    {
                        OrderId = item.id,
                        Order = db.Order.Find(item.id),
                        Date = DateTime.Now,


                    };
                    _uow.Order.UpdateOrderStatus(orderVM.Order.Id, SR.OrderDelivered);
                    db.Delivery.Add(deliveryCreate);


                }

                db.SaveChanges();
               _uow.Save();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }
        
        }

        [HttpGet]
        [Route("getDeliveryList")]
        public async Task<ActionResult<dynamic>> GetDelivey()
        {
            try
            {
                List<dynamic> deliverylist = new List<dynamic>();



                dynamic orders = db.Delivery.Select(o => new
                {

                    date = o.Date,
                    Surname = o.OrderId,

                });

                deliverylist.Add(orders);

                return deliverylist;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
            }
        }
        }
}
