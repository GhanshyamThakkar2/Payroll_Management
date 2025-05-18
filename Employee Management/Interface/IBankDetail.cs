using Employee_Management.Models;
using System.Collections.Generic;

namespace Employee_Management.Interface
{
    public interface IBankDetail
    {
        IEnumerable<BankDetail> GetAllBankDetails();
        BankDetail GetBankDetailById(int id);
        void AddBankDetail(BankDetail bankDetail);
        void UpdateBankDetail(BankDetail bankDetail);
        void DeleteBankDetail(int id);
        BankDetail GetBankDetailByEmployeeId(int employeeId);
        bool BankDetailExistsByEmployeeId(int employeeId);
    }
}
