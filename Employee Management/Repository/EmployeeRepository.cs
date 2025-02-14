using Employee_Management.Interface;
using Employee_Management.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Employee_Management.Repository
{
    public class EmployeeRepository : IEmployee
    {
        private readonly AppDBContext _context;

        public EmployeeRepository(AppDBContext context)
        {
            _context = context;
        }

        // Retrieve all employees
        public IEnumerable<Employee> GetAllEmployees()
        {
            return _context.Employees.Include(e => e.Department).ToList();
        }

        // Retrieve a single employee by ID
        public Employee GetEmployeeById(int id)
        {
            return _context.Employees.Include(e => e.Department)
                                     .FirstOrDefault(e => e.EmployeeId == id);
        }

        // Add a new employee
        public void AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        // Update an existing employee
        public void UpdateEmployee(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }

        // Delete an employee by ID
        public void DeleteEmployee(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
        }

        // Retrieve employees by department
        public IEnumerable<Employee> GetEmployeesByDepartment(int departmentId)
        {
            return _context.Employees.Where(e => e.DepartmentId == departmentId).ToList();
        }

        // Check if an employee exists by ID
        public bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
