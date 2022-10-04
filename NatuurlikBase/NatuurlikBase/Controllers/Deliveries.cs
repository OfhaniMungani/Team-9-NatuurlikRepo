using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace NatuurlikBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   
    public class Deliveries : Controller
    {
        private readonly DatabaseContext  db ;
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;
        private readonly IConfiguration _configuration;
        public Deliveries(DatabaseContext _db, IUnitOfWork uow, UserManager<ApplicationUser> userManager
          , IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory
          , IConfiguration configuration)
        {
            db = _db;
            _uow = uow;
            _userManager = userManager;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (await _userManager.IsInRoleAsync(user, "Admin")|| await _userManager.IsInRoleAsync(user, "Driver"))
                {
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        try
                        {
                            var principal = await _claimsPrincipalFactory.CreateAsync(user);
                            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);


                        }
                        catch (Exception)
                        {
                            return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server Error. Please contact support.");
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status404NotFound, "Invalid user credentials.");
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "Access Denied.");
                }
                }

           

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return Ok("Successfully logged out");
        }


        // Creating a order list. The list will contain all orders.
        [HttpGet]
        [Route("getDelivery")]
        public async Task<ActionResult<dynamic>> GetOrders()
        {
            try
            {
                List<dynamic> deliverylist = new List<dynamic>();



                dynamic orders = db.Order.Where(o=>o.Suburb.SuburbName== "Garsfontein").Where(d=>d.OrderStatus==SR.OrderDispatched).Where(c=>c.Courier.CourierName== "Natuurlik Free Delivery").Select(o => new
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
                        img=item.img,


                    };
                    _uow.Order.UpdateOrderStatus(orderVM.Order.Id, SR.OrderDelivered);
                    db.Delivery.Add(deliveryCreate);

                    string accountId = _configuration["AccountId"];
                    string authToken = _configuration["AuthToken"];
                    TwilioClient.Init(accountId, authToken);
                    var name = item.firstName;
                    var order = item.id;
                    var to = "+27" + item.phoneNumber;
                    var companyNr = "+18305216564";

                    var message = MessageResource.Create(
                        to,
                        from: companyNr,
                        body: $"Hi " + name + " your Natuurlik order #" + order + " has been delivered");

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
