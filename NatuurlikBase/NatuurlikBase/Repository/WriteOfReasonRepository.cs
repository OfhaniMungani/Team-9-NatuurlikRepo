using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class WriteOfReasonRepository : Repository<WriteOffReason>, IWriteOffReasonRepository
    {
        private DatabaseContext _db;

        public WriteOfReasonRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WriteOffReason obj)
        {
            var objFromDb = _db.WriteOffReason.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }

        }
    }
}
