using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtualAgent.Extensions;

namespace VirtualAgent.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("code")]
        public int Code { get; set; }

        [Required]
        [ForeignKey("Account")]
        [Column("account_code")]
        public int AccountCode { get; set; }

        [Required]
        [FutureDateAttribute]
        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Column("capture_date")]
        public DateTime CaptureDate { get; set; }

        [Required(ErrorMessage = "Please enter amount.")]
        [Column("amount", TypeName = "decimal(18,4)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Please enter description.")]
        [Column("description")]
        public string Description { get; set; }
    }
}
