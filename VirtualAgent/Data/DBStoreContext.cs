using Microsoft.EntityFrameworkCore;
using VirtualAgent.Models;

namespace VirtualAgent.Data
{
    public class DBStoreContext : DbContext
    {
        public DBStoreContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
