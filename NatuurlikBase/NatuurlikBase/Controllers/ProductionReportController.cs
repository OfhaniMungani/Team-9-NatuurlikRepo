using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers;
//[Authorize(Roles = SR.Role_Admin + "," + SR.Role_IM)]
public class ProductionReportController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
