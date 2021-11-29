
using System.Collections.Generic;
using VirtualAgent.Models;

namespace VirtualAgent.ViewModels
{
    public class AccountTranctionsViewModel
    {
        public Account Account { get; set; }
        public IEnumerable<Transaction> Transactions { get; set; }
    }
}