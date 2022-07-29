using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IReviewReasonRepository : IRepository<ReviewReason>
    {
        void Update(ReviewReason obj);
    }
}
