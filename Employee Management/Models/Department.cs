using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters.")]
        public string DepartmentName { get; set; }

        // Navigation Property: One department can have many employees
        public ICollection<Employee> Employees { get; set; }

        // Navigation Property: One department can have multiple designations
        public ICollection<Designation> Designations { get; set; }
    }
}
