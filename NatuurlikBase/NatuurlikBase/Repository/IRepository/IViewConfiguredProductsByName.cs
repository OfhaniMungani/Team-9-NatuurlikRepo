using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IViewConfiguredProductsByName
    {
        Task<List<ProductInventory>> ExecuteSearchAsync(string name = "");
        
    }
}