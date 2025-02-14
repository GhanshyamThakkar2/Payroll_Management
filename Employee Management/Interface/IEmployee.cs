using Employee_Management.Models;
using System.Collections.Generic;

namespace Employee_Management.Interface
{
    public interface IEmployee
    {
        IEnumerable<Employee> GetAllEmployees();
        Employee GetEmployeeById(int id);
        void AddEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        void DeleteEmployee(int id);
        IEnumerable<Employee> GetEmployeesByDepartment(int departmentId);
        bool EmployeeExists(int id);
    }
}
