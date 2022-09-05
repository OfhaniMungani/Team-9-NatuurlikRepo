using NatuurlikBase.Models;
namespace NatuurlikBase.Repository.IRepository
{
    public interface IDeliveryRepository : IRepository<Delivery>
    {
        void Update(Delivery obj);
    }
}
