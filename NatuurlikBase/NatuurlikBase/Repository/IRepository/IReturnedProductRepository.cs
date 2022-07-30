using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IReturnedProductRepository : IRepository<ReturnedProduct>
    {
        void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null);
    }
}
