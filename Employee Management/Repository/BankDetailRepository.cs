using Employee_Management.Models;
using System.Collections.Generic;
using System;
using Employee_Management.Interface;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management.Repository
{
    public class BankDetailRepository : IBankDetail
    {
        private readonly AppDBContext _context;

        public BankDetailRepository(AppDBContext context)
        {
            _context = context;
        }

        // Retrieve all bank details
        public IEnumerable<BankDetail> GetAllBankDetails()
        {
            return _context.BankDetails.Include(b => b.Employee).ToList();
        }

        public BankDetail GetBankDetailById(int id)
        {
            return _context.BankDetails.Include(b => b.Employee)
                                       .FirstOrDefault(b => b.BankDetailId == id);
        }

        // Add a new bank detail
        public void AddBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Add(bankDetail);
            _context.SaveChanges();
        }

        public void UpdateBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Update(bankDetail);
            _context.SaveChanges();
        }

        // Delete a bank detail by ID
        public void DeleteBankDetail(int id)
        {
            var bankDetail = _context.BankDetails.Find(id);
            if (bankDetail != null)
            {
                _context.BankDetails.Remove(bankDetail);
                _context.SaveChanges();
            }
        }

        public BankDetail GetBankDetailByEmployeeId(int employeeId)
        {
            return _context.BankDetails.Include(b => b.Employee)
                                       .FirstOrDefault(b => b.Employee.EmployeeId == employeeId);
        }

        // Check if a bank detail exists by ID
        public bool BankDetailExists(int id)
        {
            return _context.BankDetails.Any(b => b.BankDetailId == id);
        }
    }
}
