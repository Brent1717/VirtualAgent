using System.Collections.Generic;
using System.Linq;
using VirtualAgent.Data;
using VirtualAgent.Models;

namespace VirtualAgent.Repository.TransactionRepositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DBStoreContext _context;

        public TransactionRepository(DBStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> GetTransactions()
        {
            return _context.Transactions.ToList();
        }

        public Transaction GetTransactionByCode(int? code)
        {
            return _context.Transactions.Find(code);
        }

        public void InsertTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public void DeleteTransaction(int code)
        {
            var transaction = _context.Transactions.Find(code);
            _context.Transactions.Remove(transaction);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}