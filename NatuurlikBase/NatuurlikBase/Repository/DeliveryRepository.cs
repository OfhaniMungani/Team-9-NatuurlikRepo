using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository

{
    public class DeliveryRepository : Repository<Delivery>,IDeliveryRepository
    {
        private DatabaseContext _db;

        public DeliveryRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }


        public void Update(Delivery obj)
        {
            _db.Delivery.Update(obj);
        }
    }
}
