using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Data.DbInitilizer
{
    public class DbInitilizer : IDbInitilizer
    {

        //apply migrations if not applied
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DatabaseContext _db;
        private readonly IUnitOfWork _unitOfWork;

        public DbInitilizer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            DatabaseContext db,
            IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _unitOfWork = unitOfWork;
        }


        public async void Initialize()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (!_roleManager.RoleExistsAsync(SR.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SR.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SR.Role_MD)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SR.Role_SA)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SR.Role_IM)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SR.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SR.Role_Reseller)).GetAwaiter().GetResult();

                 _unitOfWork.Country.Add(new Country
                {
                    CountryName = "South Africa"
                });
                _unitOfWork.Save();

                _unitOfWork.Province.Add(new Province
                {
                    ProvinceName = "Gauteng",
                    CountryId = 1
                });
                _unitOfWork.Save();

                _unitOfWork.City.Add(new City
                {
                    CityName = "Pretoria",
                    ProvinceId = 1
                });
                _unitOfWork.Save();

                _unitOfWork.Suburb.Add(new Suburb
                {
                    SuburbName = "Hillcrest",
                    PostalCode= "0083",
                    CityId = 1
                });
                _unitOfWork.Save();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "u18139958@tuks.co.za",
                    Email = "u18139958@tuks.co.za",
                    FirstName = "Thenjiwe",
                    Surname = "Ntsonda",
                    PhoneNumber = "817473388",
                    StreetAddress = "18 Duxbury Rd",
                    CountryId = 1,
                    ProvinceId = 1,
                    CityId = 1,
                    SuburbId = 1,
                    UserRole="Admin",
                    EmailConfirmed=true
                }, "N@uurlik_14953").GetAwaiter().GetResult();
               
                
                ApplicationUser user = _db.User.FirstOrDefault(u => u.Email == "u18139958@tuks.co.za");

                _userManager.AddToRoleAsync(user, SR.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
