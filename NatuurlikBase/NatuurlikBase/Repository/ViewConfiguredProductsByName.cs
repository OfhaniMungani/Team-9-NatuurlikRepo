
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class ViewConfiguredProductsByName : IViewConfiguredProductsByName
    {
        private readonly IProductInventoryRepository _productInventoryRepository;
        private readonly DatabaseContext _db;

        public ViewConfiguredProductsByName(IProductInventoryRepository productInventoryRepository, DatabaseContext db)
        {
            _productInventoryRepository = productInventoryRepository;
            _db = db;
        }

        public async Task<List<ProductInventory>> ExecuteSearchAsync(string name = "")
        {
            return await _productInventoryRepository.GetConfiguredProductsAsync(name);
        }
    }
}
