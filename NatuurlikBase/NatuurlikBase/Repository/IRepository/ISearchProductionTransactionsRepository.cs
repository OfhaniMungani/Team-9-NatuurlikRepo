
using NatuurlikBase.Models;

namespace NatuurlikBase.Repository.IRepository
{
    public interface ISearchProductionTransactionsRepository
    {
        //allow users to search for production transactions using a date filter.
        Task<IEnumerable<ProductionTransaction>> ExecuteAsync(string productName, DateTime? dateFromValue, DateTime? dateToValue, ProductTransactionType? transactionType);
    }
}