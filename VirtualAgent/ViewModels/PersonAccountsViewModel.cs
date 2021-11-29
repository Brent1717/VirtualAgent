using System.Collections.Generic;
using VirtualAgent.Models;

namespace VirtualAgent.ViewModels
{
    public class PersonAccountsViewModel
    {
        public Person Person { get; set; }
        public IEnumerable<Account> Accounts { get; set; }   
    }
}