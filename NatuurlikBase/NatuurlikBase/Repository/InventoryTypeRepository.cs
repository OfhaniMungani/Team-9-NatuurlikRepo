using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
namespace NatuurlikBase.Repository
{
    public class InventoryTypeRepository : Repository<InventoryType>, IInventoryType
    {
        private DatabaseContext _db;

        public InventoryTypeRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(InventoryType obj)
        {
            _db.InventoryType.Update(obj);
        }
    }
}
