using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Assessment.Models;

namespace Assessment.Data
{
    public partial class AssessmentContext : DbContext
    {
        public AssessmentContext()
        {
        }

        public AssessmentContext(DbContextOptions<AssessmentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Lease> Leases { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("User ID=postgres; Password=Test1234; Host=localhost; Port=5432; Database=Assessment;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lease>(entity =>
            {
                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Leases)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("leases_unit_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
