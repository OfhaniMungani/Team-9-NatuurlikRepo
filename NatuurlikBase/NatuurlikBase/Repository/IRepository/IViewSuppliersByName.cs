using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IViewSuppliersByName
    {
        Task<List<Supplier>> SearchSupplierAsync(string name = "");
    }
}