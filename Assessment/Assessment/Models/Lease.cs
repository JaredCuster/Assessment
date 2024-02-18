using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Models
{
    [Table("leases")]
    public partial class Lease
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("move_in")]
        public DateTime MoveIn { get; set; }
        [Column("move_out")]
        public DateTime? MoveOut { get; set; }
        [Column("resident_name")]
        [StringLength(240)]
        public string ResidentName { get; set; } = null!;
        [Column("unit_id")]
        public int UnitId { get; set; }

        [ForeignKey("UnitId")]
        [InverseProperty("Leases")]
        public virtual Unit Unit { get; set; } = null!;
    }
}
