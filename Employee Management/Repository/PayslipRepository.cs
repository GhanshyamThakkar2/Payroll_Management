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
        public IEnumerable<Payslip> GetAllPayslips()
        {
            return _context.Payslips.Include(p => p.Employee).ToList();
        }

        public Payslip GetPayslipById(int payslipId)
        {
            return _context.Payslips.Include(p => p.Employee)
                                    .FirstOrDefault(p => p.PayslipId == payslipId);
        }
        public void AddPayslip(Payslip payslip)
        {
            _context.Payslips.Add(payslip);
            _context.SaveChanges();
        }

        public void UpdatePayslip(Payslip payslip)
        {
            _context.Payslips.Update(payslip);
            _context.SaveChanges();
        }
        public void DeletePayslip(int payslipId)
        {
            var payslip = _context.Payslips.Find(payslipId);
            if (payslip != null)
            {
                _context.Payslips.Remove(payslip);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Payslip> GetPayslipsByEmployeeId(int employeeId)
        {
            return _context.Payslips.Include(p => p.Employee)
                                    .Where(p => p.EmployeeId == employeeId)
                                    .ToList();
        }
        public IEnumerable<Payslip> GetPayslipsByDepartmentId(int departmentId)
        {
            return _context.Payslips.Include(p => p.Employee)
                                    .ThenInclude(e => e.Department)
                                    .Where(p => p.Employee.Department.DepartmentId == departmentId)
                                    .ToList();
        }

        public IEnumerable<Payslip> GetPayslipsByYearAndMonth(int year, string month)
        {
            return _context.Payslips.Include(p => p.Employee)
                                    .Where(p => p.Year == year && p.Month == month)
                                    .ToList();
        }

        public bool PayslipExists(int payslipId)
        {
            return _context.Payslips.Any(p => p.PayslipId == payslipId);
        }
    }
}
