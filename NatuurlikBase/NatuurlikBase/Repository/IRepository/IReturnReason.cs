using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IReturnReason:IRepository<ReturnReason>
    {
        void Update(ReturnReason obj);

    }
    
}
