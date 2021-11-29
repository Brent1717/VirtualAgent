using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualAgent.Models
{
    public class Account
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }

        [ForeignKey("Person")]
        [Column("person_code")]
        public int PersonCode { get; set; }

        [Column("account_number")]
        public string AccountNumber { get; set; }

        [Column("outstanding_balance", TypeName = "decimal(18,4)")]
        public decimal OutstandingBalance { get; set; }
    }
}
