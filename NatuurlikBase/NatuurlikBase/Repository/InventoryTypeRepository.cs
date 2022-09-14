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
            var objFromDb = _db.InventoryType.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.InventoryTypeName = obj.InventoryTypeName;
            }
        }
    }
}
