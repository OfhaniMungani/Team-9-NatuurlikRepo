using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IWriteOffReasonRepository : IRepository<WriteOffReason>
    {
        void Update(WriteOffReason obj);
    }
}
