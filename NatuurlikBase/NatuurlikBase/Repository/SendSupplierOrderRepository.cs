
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class SendSupplierOrderRepository : ISendSupplierOrderRepository
    {
        private readonly ISupplierOrderRepository _supplierOrderRepository;

        public SendSupplierOrderRepository(ISupplierOrderRepository supplierOrderRepository)
        {
            _supplierOrderRepository = supplierOrderRepository;
        }

        public async Task ExecuteAsync(Supplier supplier)
        {
            await _supplierOrderRepository.SendSupplierOrderAsync(supplier);
        }
    }
}
