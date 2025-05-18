using Employee_Management.Interface;
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
        public DbSet<Designation> Designations { get; set; }
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

            // Designation and Department (One-to-Many)
            modelBuilder.Entity<Designation>()
                .HasOne(e=>e.Department)
                .WithMany(d=>d.Designations)
                .HasForeignKey(e => e.DepartmentId);

            // Employee and Designation (One-to-Many)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Designation)
                .WithMany() // No navigation property in Designation
                .HasForeignKey(e => e.DesignationId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents cascade delete issues


            modelBuilder.Entity<BankDetail>()
                .HasOne(b => b.Employee)
                .WithOne()
                .HasForeignKey<BankDetail>(b => b.EmployeeId) // EmployeeId is FK in BankDetail
                .OnDelete(DeleteBehavior.Cascade); // Deletes BankDetail if Employee is deleted

            // Employee and Payslip (One-to-Many)
            modelBuilder.Entity<Payslip>()
                .HasOne(p => p.Employee)
                .WithMany(e => e.Payslips)
                .HasForeignKey(p => p.EmployeeId);
        }
    }
}
