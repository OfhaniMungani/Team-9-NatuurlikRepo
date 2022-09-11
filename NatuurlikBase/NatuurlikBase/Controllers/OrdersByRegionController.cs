using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers;
//[Authorize(Roles = SR.Role_Admin)]
public class OrdersByRegionController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
