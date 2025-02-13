using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Models
{
    public class DepartmentDBContext : DbContext
    {
        public DepartmentDBContext(DbContextOptions<DepartmentDBContext> options) : base (options)
        {
        }
        //Dbset for Department
        public DbSet<Department> Departments { get; set; }

        //Dbset for Employee
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.DepartmentId);
                entity.Property(d => d.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(100);//Max Length for Department Name
                entity.Property(d => d.Designation)
                    .HasMaxLength(100);//Max Length for Designation

            });
            // Configure Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId); // Primary key
                entity.Property(e => e.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Email)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(e => e.Phone)
                      .HasMaxLength(15);

                // Configure one-to-many relationship between Department and Employee
                entity.HasOne(e => e.Department)
                      .WithMany(d => d.Employees)
                      .HasForeignKey("DepartmentId") // Define the foreign key in Employee
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete employees when a department is deleted
            });
        }
    }
}
