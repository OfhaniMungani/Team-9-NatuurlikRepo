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
            var objFromDb = _db.InventoryProcured.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.InvoiceNo = obj.InvoiceNo;
                objFromDb.InvoiceFile = obj.InvoiceFile;
                objFromDb.SupplierId = obj.SupplierId;
                objFromDb.DateLogged = obj.DateLogged;
                objFromDb.ItemID = obj.ItemID;
                objFromDb.QuantityReceived = obj.QuantityReceived;
            }
        }
    
    }
}
