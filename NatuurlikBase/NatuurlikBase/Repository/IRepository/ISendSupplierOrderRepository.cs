using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface ISendSupplierOrderRepository
    {
        Task ExecuteAsync(Supplier supplier);
    }
}