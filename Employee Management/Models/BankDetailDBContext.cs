using Microsoft.EntityFrameworkCore;
using System;

namespace Employee_Management.Models
{
    public class BankDetailDBContext : DbContext
    {
        public BankDetailDBContext(DbContextOptions<BankDetailDBContext> options) : base(options)
        {
        }

        // DbSet for BankDetail
        public DbSet<BankDetail> BankDetails { get; set; }

        // DbSet for other entities
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Login> Logins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure BankDetail entity
            modelBuilder.Entity<BankDetail>(entity =>
            {
                entity.HasKey(b => b.BankDetailId); // Primary key of BankDetail Model
                entity.Property(b => b.AccountHolderName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(b => b.AccountNumber)
                      .IsRequired()
                      .HasMaxLength(20);
                entity.Property(b => b.IFSCCode)
                      .IsRequired()
                      .HasMaxLength(15);
                entity.Property(b => b.BankName)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(b => b.Branch)
                      .HasMaxLength(100);
            });

            // Configure relationships one to one between Employee and bank detail
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.BankDetail)
                .WithOne()
                .HasForeignKey<Employee>(e => e.BankDetail) // Assuming Employee has BankDetailId as FK
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
