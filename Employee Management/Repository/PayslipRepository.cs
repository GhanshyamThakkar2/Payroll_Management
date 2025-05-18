using Employee_Management.Interface;
using Employee_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Employee_Management.Repository
{
    public class PayslipRepository : IPayslip
    {
        private readonly AppDBContext _context;

        public PayslipRepository(AppDBContext context)
        {
            _context = context;
        }

        // Get all payslips with employee details
        public IEnumerable<Payslip> GetAllPayslips()
        {
            return _context.Payslips.Include(p => p.Employee).ToList();
        }

        // Get a payslip by ID (optimized with Find)
        public Payslip GetPayslipById(int payslipId)
        {
            return _context.Payslips.Find(payslipId);
        }

        // Add a new payslip
        public void AddPayslip(Payslip payslip)
        {
            _context.Payslips.Add(payslip);
            _context.SaveChanges();
        }

        // Update a payslip
        public void UpdatePayslip(Payslip payslip)
        {
            _context.Payslips.Update(payslip);
            _context.SaveChanges();
        }

        // Delete a payslip by ID
        public void DeletePayslip(int payslipId)
        {
            var payslip = _context.Payslips.Find(payslipId);
            if (payslip != null)
            {
                _context.Payslips.Remove(payslip);
                _context.SaveChanges();
            }
        }

        // Get all payslips for a specific employee
        public IEnumerable<Payslip> GetPayslipsByEmployeeId(int employeeId)
        {
            return _context.Payslips.Include(p => p.Employee)
                                    .Where(p => p.EmployeeId == employeeId)
                                    .ToList();
        }

        // Get all payslips for a specific department
        public IEnumerable<Payslip> GetPayslipsByDepartmentId(int departmentId)
        {
            return _context.Payslips.Include(p => p.Employee.Department)
                                    .Where(p => p.Employee.Department.DepartmentId == departmentId)
                                    .ToList();
        }

        // Get payslips for a specific year and month (Updated to use int for month)
        public IEnumerable<Payslip> GetPayslipsByYearAndMonth(int year, int month)
        {
            return _context.Payslips.Include(p => p.Employee)
                                    .Where(p => p.Year == year && p.Month == month)
                                    .ToList();
        }

        // Check if a payslip exists by ID
        public bool PayslipExists(int payslipId)
        {
            return _context.Payslips.Any(p => p.PayslipId == payslipId);
        }

        // Prevent duplicate payslips for the same employee, year, and month
        public bool PayslipExistsForEmployee(int employeeId, int year, int month)
        {
            return _context.Payslips.Any(p => p.EmployeeId == employeeId && p.Year == year && p.Month == month);
        }
    }
}
