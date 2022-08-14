using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using System.Security.Claims;

namespace NatuurlikBase.Controllers
{
    public class ResellerReport : Controller
    {

        private readonly DatabaseContext _context;

        public ResellerReport(DatabaseContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {

            var claimsId = (ClaimsIdentity)User.Identity;
            var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
            var actorName = _context.Users.Where(x => x.Id == claim.Value).FirstOrDefault();

            ViewBag.ActorName = actorName.FirstName;
            ViewBag.Surname = actorName.Surname;

            IEnumerable<ApplicationUser> users = (from user in _context.Users
                                                  join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                                  join role in _context.Roles on userRole.RoleId equals role.Id
                                                  where role.Name == "Reseller"
                                                  select user)
                               .ToList();

            ViewBag.count = users.Count();

            return View(users);
        }
    }
}
