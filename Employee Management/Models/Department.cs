using System.Collections.Generic;

namespace Employee_Management.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public string Designation { get; set; }
    }

}
