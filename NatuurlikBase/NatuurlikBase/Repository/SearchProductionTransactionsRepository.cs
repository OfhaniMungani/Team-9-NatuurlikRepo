
using NatuurlikBase.Models;
using NatuurlikBase.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NatuurlikBase.Repository
{
    public class SearchProductionTransactionsRepository : ISearchProductionTransactionsRepository
    {
        private readonly IProductionTransactionRepository _productionTransactionRepository;

        public SearchProductionTransactionsRepository(IProductionTransactionRepository productionTransactionRepository)
        {
            _productionTransactionRepository = productionTransactionRepository;
        }

        public async Task<IEnumerable<ProductionTransaction>> ExecuteAsync(
            string productName,
            DateTime? dateFrom,
            DateTime? dateTo,
            ProductTransactionType? transactionType)
        {
            return await _productionTransactionRepository.ProductionTransactions(
                productName,
                dateFrom,
                dateTo,
                transactionType
                );
        }
    }
}
