using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IVATRepository : IRepository<VAT>
    {
        void Update(VAT obj);
    }
}
