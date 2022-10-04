
using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class VATRepository : Repository<VAT>, IVATRepository
    {
        private DatabaseContext _db;

        public VATRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(VAT obj)
        {
            var objFromDb = _db.VAT.FirstOrDefault(u => u.Id == obj.Id);
            if(objFromDb != null)
            {
                objFromDb.CreatedDate = obj.CreatedDate;
                objFromDb.VATStatus = obj.VATStatus;
                objFromDb.VATFactor = obj.VATFactor;
                objFromDb.VATPercentage = obj.VATPercentage;
            }
        }
    }
}
