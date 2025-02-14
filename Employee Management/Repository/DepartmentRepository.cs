using Employee_Management.Interface;
using Employee_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Employee_Management.Repository
{
    public class DepartmentRepository : IDepartment
    {
        private readonly AppDBContext _context;

        public DepartmentRepository(AppDBContext context)
        {
            _context = context;
        }
        public IEnumerable<Department> GetAllDepartments()
        {
            return _context.Departments.Include(d => d.Employees).ToList();
        }
        public Department GetDepartmentById(int id)
        {
            return _context.Departments.Include(d => d.Employees)
                                       .FirstOrDefault(d => d.DepartmentId == id);
        }
        public void AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            _context.SaveChanges();
        }

        public void UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);
            _context.SaveChanges();
        }
        public void DeleteDepartment(int id)
        {
            var department = _context.Departments.Find(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Employee> GetEmployeesByDepartmentId(int departmentId)
        {
            return _context.Employees.Where(e => e.DepartmentId == departmentId).ToList();
        }

        public bool DepartmentExists(int id)
        {
            return _context.Departments.Any(d => d.DepartmentId == id);
        }
    }
}
