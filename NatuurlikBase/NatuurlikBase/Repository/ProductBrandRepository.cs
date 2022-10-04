﻿using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class ProductBrandRepository : Repository<ProductBrand>, IProductBrandRepository
    {
        private DatabaseContext _db;

        public ProductBrandRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }


        public void Update(ProductBrand obj)
        {
            var objFromDb = _db.Brands.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }
        }
    }
}
