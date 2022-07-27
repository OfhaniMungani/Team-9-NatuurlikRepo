
using NatuurlikBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository.IRepository
{
    public interface ISupplierOrderRepository
    {
        Task<List<Supplier>> GetSuppliersByNameAsync(string name);
        Task<Supplier> GetSupplierByIdAsync(int supplierId);
        Task SendSupplierOrderAsync(Supplier supplier);

       
    }
}
