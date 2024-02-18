using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Models
{
    [Table("units")]
    public partial class Unit
    {
        public Unit()
        {
            Leases = new HashSet<Lease>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("max_capacity")]
        public int MaxCapacity { get; set; }

        [InverseProperty("Unit")]
        public virtual ICollection<Lease> Leases { get; set; }
    }
}
