
using Microsoft.EntityFrameworkCore;
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
    public class SupplierOrderRepository : ISupplierOrderRepository
    {
        private readonly DatabaseContext _db;

        public SupplierOrderRepository(DatabaseContext db)
        {
            _db = db;
        }


        public async Task<Supplier> GetSupplierByIdAsync(int supplierId)
        {

            return await _db.Suppliers.Include(x => x.SupplierInventory)
                .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.Id == supplierId);
        }

        public async Task<List<Supplier>> GetSuppliersByNameAsync(string supplierName)
        {
            return await _db.Suppliers.Where(x => (x.CompanyName.ToLower().IndexOf(supplierName.ToLower()) >= 0 ||
                                                    string.IsNullOrWhiteSpace(supplierName))).ToListAsync();
        }


        public async Task SendSupplierOrderAsync(Supplier supplier)
        {
            var supplierOrderInventory = await _db.Suppliers.FindAsync(supplier.Id);
            if (supplierOrderInventory != null)
            {
                supplierOrderInventory.CompanyName = supplier.CompanyName;
                supplierOrderInventory.EmailAddress = supplier.EmailAddress;
                //supplierOrderInventory.SupplierOrder = supplier.SupplierOrder;
                await _db.SaveChangesAsync();

            }

        }
    }
}
