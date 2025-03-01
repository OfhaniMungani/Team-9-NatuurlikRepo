﻿using NatuurlikBase.Data;
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;

namespace NatuurlikBase.Repository
{
    public class ReviewReasonRepository : Repository<ReviewReason>, IReviewReasonRepository
    {
        private DatabaseContext _db;

        public ReviewReasonRepository(DatabaseContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ReviewReason obj)
        {
            var objFromDb = _db.ReviewReason.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
            }

        }

    }
}

