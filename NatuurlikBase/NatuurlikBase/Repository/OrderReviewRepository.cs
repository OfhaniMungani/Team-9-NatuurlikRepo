using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class OrderReviewRepository :Repository<OrderReview>, IOrderReviewcs
    {
        private DatabaseContext _db;

    public OrderReviewRepository(DatabaseContext db) : base(db)
    {
        _db = db;
    }


    public void Update(OrderReview obj)
    {
        _db.OrderReview.Update(obj);
    }
    }
}

