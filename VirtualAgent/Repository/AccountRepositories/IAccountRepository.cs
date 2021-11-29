using System.Collections.Generic;
using VirtualAgent.Models;

namespace VirtualAgent.Repository.AccountRepositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAccounts();
        Account GetAccountByCode(int? code);
        void DeleteAccount(int code);
        void InsertAccount(Account account);
        void Save();
    }
}