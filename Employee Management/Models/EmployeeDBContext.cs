using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Models
{
    public class EmployeeDBContext : DbContext
    {
        public EmployeeDBContext(DbContextOptions<EmployeeDBContext> options)
            :base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
