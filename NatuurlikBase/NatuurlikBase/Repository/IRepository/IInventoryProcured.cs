using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IInventoryProcured : IRepository<InventoryProcured>
    {
        void Update(InventoryProcured obj);
    
    }
}
