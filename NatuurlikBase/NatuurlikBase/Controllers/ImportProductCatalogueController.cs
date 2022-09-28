using Microsoft.AspNetCore.Mvc;
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using Newtonsoft.Json;

namespace NatuurlikBase.Controllers
{
    public class ImportProductCatalogueController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly DatabaseContext _context;

        public ImportProductCatalogueController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, DatabaseContext context)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Import(IFormFile file)
        {

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string json = reader.ReadToEnd();
                    List<Product> items = JsonConvert.DeserializeObject<List<Product>>(json);
                    foreach (var prod in items)
                    {
                        _unitOfWork.Product.Add(prod);
                    }
                }
                _unitOfWork.Save();
                TempData["success"] = "Products imported successfully";
            }

            catch
            {
                TempData["error"] = "An error occurred when importing products.";
            }
          
            return RedirectToAction(nameof(Index));
        }
    }
}
