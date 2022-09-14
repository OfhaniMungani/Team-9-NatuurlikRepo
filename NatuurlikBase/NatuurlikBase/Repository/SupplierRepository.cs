using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        private DatabaseContext _db;

        public SupplierRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Supplier obj)
        {
            var objFromDb = _db.Suppliers.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.CompanyName = obj.CompanyName;
                objFromDb.PhoneNumber = obj.PhoneNumber;
                objFromDb.EmailAddress = obj.EmailAddress;
                objFromDb.StreetAddress = obj.StreetAddress;
                objFromDb.CountryId = obj.CountryId;
                objFromDb.ProvinceId = obj.ProvinceId;
                objFromDb.CityId = obj.CityId;
                objFromDb.SuburbId = obj.SuburbId;
            }

        }

    }
}
