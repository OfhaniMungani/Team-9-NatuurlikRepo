using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

namespace NatuurlikBase.Controllers;

[Authorize(Roles = SR.Role_Admin)]

public class ViewDeliveries : Controller
{
    private readonly DatabaseContext _context;
    public ViewDeliveries(DatabaseContext context)
    {
        _context = context;

    }
    public async Task<IActionResult> Index()
    {
        var databaseContext = _context.Delivery;
        return View(await databaseContext.ToListAsync());
    }
}
