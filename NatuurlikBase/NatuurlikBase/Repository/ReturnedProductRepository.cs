using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class ReturnedProductRepository : Repository<ReturnedProduct>, IReturnedProductRepository
    {
        private DatabaseContext _db;

        public ReturnedProductRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void UpdateOrderStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            //Retrieve the order from the database.
            var order = _db.Order.FirstOrDefault(u => u.Id == id);

            if (order != null)
            {
                order.OrderStatus = orderStatus;

                if (paymentStatus != null)
                {
                    order.OrderPaymentStatus = paymentStatus;
                }
            }
        }
    }
}
