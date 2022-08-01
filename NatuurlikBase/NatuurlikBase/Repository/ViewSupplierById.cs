
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class ViewSupplierById : IViewSupplierById
    {
        private readonly ISupplierOrderRepository _supplierOrderRepository;

        public ViewSupplierById(ISupplierOrderRepository supplierOrderRepository)
        {
            _supplierOrderRepository = supplierOrderRepository;
        }

        public async Task<Supplier> ExecuteAsync(int supplierId)
        {
            return await _supplierOrderRepository.GetSupplierByIdAsync(supplierId);
        }
    }
}
