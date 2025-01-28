using Employee_Management.Models;
using System.Collections.Generic;

namespace Employee_Management.Interface
{
    public interface IDepartment
    {
        IEnumerable<Department> GetAllDepartments();
        Department GetDepartmentById(int id);
        void AddDepartment(Department department);
        void UpdateDepartment(Department department);
        void DeleteDepartment(int id);
        IEnumerable<Employee> GetEmployeesByDepartmentId(int departmentId);
        bool DepartmentExists(int id);
    }
}
