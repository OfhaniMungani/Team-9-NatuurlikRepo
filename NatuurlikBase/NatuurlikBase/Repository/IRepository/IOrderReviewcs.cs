using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IOrderReviewcs : IRepository<OrderReview>
    {
        void Update(OrderReview obj);
    }
    
}
