using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class UserRecord
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [MaxLength(10)]
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Tel { get; set; }
        
        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

    }
}