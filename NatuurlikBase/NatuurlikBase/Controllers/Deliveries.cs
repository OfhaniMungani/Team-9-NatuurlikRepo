﻿using Microsoft.AspNetCore.Authentication;
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
