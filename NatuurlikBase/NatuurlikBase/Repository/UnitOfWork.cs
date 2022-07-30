﻿
using NatuurlikBase.Data;
using NatuurlikBase.Repository;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class UnitOfWork : IUnitOfWork
    {

        private DatabaseContext _db;

        public UnitOfWork(DatabaseContext db)
        {
            _db = db;
            Country = new CountryRepository(_db);
            Province = new ProvinceRepository(_db);
            City = new CityRepository(_db);
            Suburb = new SuburbRepository(_db);
            User = new UserRepository(_db);
            Product = new ProductRepository(_db);
            Brand = new ProductBrandRepository(_db);
            Category = new ProductCategoryRepository(_db);
            Supplier = new SupplierRepository(_db);
            UserCart = new UserCartRepository(_db);
            Order = new OrderRepository(_db);
            OrderLine = new OrderLineRepository(_db);
            Courier = new CourierRepository(_db);
            OrderQuery = new OrderQueryRepository(_db);
            QueryReason = new QueryReasonRepository(_db);
            ReviewReason = new ReviewReasonRepository(_db);
            OrderReview = new OrderReviewRepository(_db);
            ReturnedProduct = new ReturnedProductRepository(_db);


        }
        public ICountryRepository Country { get; private set; }
        public IProvinceRepository Province { get; private set; }
        public ICityRepository City { get; private set; }
        public ISuburbRepository Suburb { get; private set; }
        public IUserRepository User { get; private set; }
        public IProductRepository Product { get; private set; }
        public IProductBrandRepository Brand { get; private set; }
        public IProductCategoryRepository Category { get; private set; }
        public ISupplierRepository Supplier { get; private set; }

        public IUserCartRepository UserCart { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IOrderLineRepository OrderLine { get; private set; }
        public ICourierRepository Courier { get; private set; }
        public IOrderQueryRepository OrderQuery { get; private set; }

        public IQueryReasonRepository QueryReason { get; private set; }
        public IReviewReasonRepository ReviewReason { get; private set; }
        public IOrderReviewcs OrderReview { get; private set; }

        public IReturnedProductRepository ReturnedProduct { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
