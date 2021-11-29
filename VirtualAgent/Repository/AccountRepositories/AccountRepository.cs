
using System.Collections.Generic;
using System.Linq;
using VirtualAgent.Data;
using VirtualAgent.Models;
using VirtualAgent.Repository.AccountRepositories;

namespace VirtualAgent.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DBStoreContext _context;

        public AccountRepository(DBStoreContext context)
        {
            _context = context;
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _context.Accounts.ToList();
        }

        public Account GetAccountByCode(int? code)
        {
            return _context.Accounts.Find(code);
        }

        public void InsertAccount(Account account)
        {
            _context.Accounts.Add(account);
        }

        public void DeleteAccount(int studentID)
        {
            var account = _context.Accounts.Find(studentID);
            _context.Accounts.Remove(account);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}