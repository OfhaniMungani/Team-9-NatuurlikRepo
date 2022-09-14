using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
    {
        private DatabaseContext _db;

        public ProductCategoryRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }


        public void Update(ProductCategory obj)
        {
            var objFromDb = _db.Categories.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }
        }
    }
}
