using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Models
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<BankDetail> BankDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Employee and Department (One-to-Many)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId);

            // Employee and BankDetail (One-to-One)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.BankDetail)
                .WithOne(b => b.Employee)
                .HasForeignKey<Employee>(e => e.BankDetailId);

            // Employee and Payslip (One-to-Many)
            modelBuilder.Entity<Payslip>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Payslip)
                .HasForeignKey(p => p.EmployeeId);
        }
    }
}
