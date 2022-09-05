using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IInventoryType: IRepository< InventoryType>
    {
        void Update(InventoryType obj);
    
    }
}
