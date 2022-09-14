using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace NatuurlikBase.Controllers;

//[Authorize(Roles = SR.Role_Admin)]

public class AuditController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly DatabaseContext _db;
    public AuditController(IUnitOfWork unitOfWork, DatabaseContext db)
    {
        _unitOfWork = unitOfWork;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Audit> auditTransactions =  await _db.Audit.ToListAsync();
        return View(auditTransactions);
    }

}
