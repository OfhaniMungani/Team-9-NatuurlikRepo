using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers
{
    public class CustomerReport : Controller
    {

        private readonly DatabaseContext _context;

        public CustomerReport(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationUser> users = (from user in _context.Users
                                                  join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                                  join role in _context.Roles on userRole.RoleId equals role.Id
                                                  where role.Name == "Customer"
                                                  select user)
                                                    .ToList();
            ViewBag.count = users.Count();

            return View(users);
        }
    }
}
