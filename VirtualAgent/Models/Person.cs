
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VirtualAgent.Models
{
    public class Person
    {
        [Key]
        [Column("code")]
        public int Code { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; }

        [Column("surname")]
        [Required]
        public string Surname { get; set; }

        [Required]
        [Column("id_number")]
        public string IDNumber { get; set; }
    }
}
