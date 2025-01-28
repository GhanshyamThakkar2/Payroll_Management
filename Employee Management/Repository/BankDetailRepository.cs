using Employee_Management.Models;
using System.Collections.Generic;
using System;

namespace Employee_Management.Repository
{
    public class BankDetailRepository
    {
        private readonly BankDetailRepository _context;

        public BankDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieve all bank details
        public IEnumerable<BankDetail> GetAllBankDetails()
        {
            return _context.BankDetails.ToList();
        }

        // Retrieve a single bank detail by ID
        public BankDetail GetBankDetailById(int bankDetailId)
        {
            return _context.BankDetails.FirstOrDefault(b => b.BankDetailId == bankDetailId);
        }

        // Add a new bank detail
        public void AddBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Add(bankDetail);
            _context.SaveChanges();
        }

        // Update an existing bank detail
        public void UpdateBankDetail(BankDetail bankDetail)
        {
            _context.BankDetails.Update(bankDetail);
            _context.SaveChanges();
        }

        // Delete a bank detail by ID
        public void DeleteBankDetail(int bankDetailId)
        {
            var bankDetail = _context.BankDetails.Find(bankDetailId);
            if (bankDetail != null)
            {
                _context.BankDetails.Remove(bankDetail);
                _context.SaveChanges();
            }
        }

        // Retrieve a bank detail by employee ID (if linked to an employee)
        public BankDetail GetBankDetailByEmployeeId(int employeeId)
        {
            return _context.Employees
                .Include(e => e.BankDetail) // Include related bank detail
                .FirstOrDefault(e => e.EmployeeId == employeeId)?.BankDetail;
        }

        // Check if a bank detail exists by ID
        public bool BankDetailExists(int bankDetailId)
        {
            return _context.BankDetails.Any(b => b.BankDetailId == bankDetailId);
        }
    }
}
