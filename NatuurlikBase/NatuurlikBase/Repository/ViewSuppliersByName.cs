
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class ViewSuppliersByName : IViewSuppliersByName
    {
        private readonly ISupplierOrderRepository _supplierOrderRepository;

        public ViewSuppliersByName(ISupplierOrderRepository supplierOrderRepository)
        {
            _supplierOrderRepository = supplierOrderRepository;
        }

        public async Task<List<Supplier>> SearchSupplierAsync (string supplierName = "")
        {
            return await _supplierOrderRepository.GetSuppliersByNameAsync(supplierName);
        }
    }
}
