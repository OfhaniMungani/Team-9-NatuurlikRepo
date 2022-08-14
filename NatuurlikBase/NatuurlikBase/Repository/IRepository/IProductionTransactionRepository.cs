using NatuurlikBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository.IRepository
{
    public interface IProductionTransactionRepository
    {
        Task<IEnumerable<ProductionTransaction>> ProductionTransactions(string productName, DateTime? dateFromValue, DateTime? dateToValue, ProductTransactionType? transactionType);

        Task ProduceAsync(Product product, int quantity, string actor);

    }
}
