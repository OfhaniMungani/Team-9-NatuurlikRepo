using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class ReturnReasonRepository : Repository<ReturnReason>, IReturnReason
    {
        private DatabaseContext _db;

        public ReturnReasonRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ReturnReason obj)
        {
            var objFromDb = _db.ReturnReason.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.ReturnReasonName = obj.ReturnReasonName;
            }

        }
    }
}
