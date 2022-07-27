

using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository

{
    public interface IViewSupplierById
    {
        Task<Supplier> ExecuteAsync(int supplierId);
    }
}