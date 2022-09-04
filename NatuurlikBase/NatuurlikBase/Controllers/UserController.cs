﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using NatuurlikBase.ViewModels;

namespace NatuurlikBase.Controllers
{   
    public class UserController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly DatabaseContext _db;

        public UserController(IUnitOfWork unitOfWork, DatabaseContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            IEnumerable<Country> countryList = _unitOfWork.Country.GetAll();
            return View(countryList);

        }
        public ActionResult GetProvince(int countryId)
        {
            return Json(_db.Province.Where(x => x.CountryId == countryId).Select(x => new
            {
                Text = x.ProvinceName,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList());
        }


        [HttpGet]
        public ActionResult GetCity(int provinceId)
        {
            return Json(_db.City.Where(x => x.ProvinceId == provinceId).Select(x => new
            {
                Text = x.CityName,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList());
        }

        [HttpGet]
        public ActionResult GetSuburb(int cityId)
        {
            return Json(_db.Suburb.Where(x => x.CityId == cityId).Select(x => new
            {
                Text = x.SuburbName,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList());
        }
        //GET
        public IActionResult Upsert(string? id)
        {
            UserVM userVM = new()
            {
                User = new(),
                CountryList = _unitOfWork.Country.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CountryName,
                    Value = i.Id.ToString()
                })
                ,
                ProvinceForCountryList = _unitOfWork.Province.GetAll().Select(i => new SelectListItem
                {
                    Text = i.ProvinceName,
                    Value = i.Id.ToString()
                }),
                CityForProvinceList = _unitOfWork.City.GetAll().Select(i => new SelectListItem
                {
                    Text = i.CityName,
                    Value = i.Id.ToString()
                }),
                SuburbForCityList = _unitOfWork.Suburb.GetAll().Select(i => new SelectListItem
                {
                    Text = i.SuburbName,
                    Value = i.Id.ToString()
                }),
            };

            if (id == null)
            {
                return View(userVM);
            }
            else
            {
                userVM.User = _unitOfWork.User.GetFirstOrDefault(u => u.Id == id);
                return View(userVM);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //protect against anti forgery token attacks.
        public IActionResult Upsert(UserVM obj)
        {
            
            if(ModelState.IsValid)
            {

              
                if(obj.User.Id == null)
                {
                    _unitOfWork.User.Add(obj.User);
                }
                else
                {
                    _unitOfWork.User.Update(obj.User);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "User Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(obj);
       
        }


        

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objList = _unitOfWork.User.GetAll(includeProperties: "");
            return Json(new {data = objList });
        }
        [HttpDelete]
        
        public IActionResult Delete(string? id)
        {

            //Has order associated?
            var hasFk = _unitOfWork.Order.GetAll().Any(x => x.ApplicationUserId == id);

            if(hasFk)
            {
                TempData["Delete"] = "User cannot be deleted since their profile is associate with an Order";
                return Json(new { success = false, message = "User cannot be deleted since their profile is associate with an Order" });
            }
            else
            {
                var obj = _unitOfWork.User.GetFirstOrDefault(u => u.Id == id);
                if (obj == null)
                {
                    return Json(new { success = false, message = "An error occured while deleting" });
                }
                _unitOfWork.User.Remove(obj);
                TempData["successDelete"] = "Product deleted successfully";
                _unitOfWork.Save();
                return Json(new { success = true, message = "User Deleted Successfully" });
            }

        }
        #endregion

    }

}

