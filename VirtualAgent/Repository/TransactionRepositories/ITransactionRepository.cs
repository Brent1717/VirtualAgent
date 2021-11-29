using System.Collections.Generic;
using VirtualAgent.Models;

namespace VirtualAgent.Repository.TransactionRepositories
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> GetTransactions();
        Transaction GetTransactionByCode(int? code);
        void DeleteTransaction(int code);
        void InsertTransaction(Transaction transaction);
        void Save();   
    }
}