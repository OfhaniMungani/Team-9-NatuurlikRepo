using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class InventoryProcuredRepository : Repository<InventoryProcured>, IInventoryProcured
    {
        private DatabaseContext _db;

        public InventoryProcuredRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InventoryProcured obj)
        {
            _db.InventoryProcured.Update(obj);
        }
    
    }
}
