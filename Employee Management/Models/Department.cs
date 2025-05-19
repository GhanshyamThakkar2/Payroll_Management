using System.Collections.Generic;

namespace Employee_Management.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        //Navigation Property
        public ICollection<Employee> Employees { get; set; }
        // Navigation Property: One department can have multiple designations
        public ICollection<Designation> Designations { get; set; }
    }
}