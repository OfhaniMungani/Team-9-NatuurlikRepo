﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NatuurlikBase.Data;
using NatuurlikBase.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NatuurlikBase.Controllers;

[Authorize(Roles = SR.Role_Admin + "," + SR.Role_IM)]
public class InventoryReportController : Controller
{
    private readonly DatabaseContext db;
    public InventoryReportController(DatabaseContext _db)
    {
        db = _db;
    }
    public IActionResult Index()
    {
        var claimsId = (ClaimsIdentity)User.Identity;
        var claim = claimsId.FindFirst(ClaimTypes.NameIdentifier);
        var actorName = db.Users.Where(x => x.Id == claim.Value).FirstOrDefault();

        ViewBag.ActorName = actorName.FirstName;
        ViewBag.Surname = actorName.Surname;

        IEnumerable<InventoryItem> inventoryItems = db.InventoryItem.Include(b => b.InventoryType).ToList();
        ViewBag.total = inventoryItems.Sum(o => o.QuantityOnHand);
        return View(inventoryItems);
    }
}

