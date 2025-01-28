using Employee_Management.Models;
using System.Collections.Generic;

namespace Employee_Management.Interface
{
    public interface IPayslip
    {
        IEnumerable<Payslip> GetAllPayslips();
        Payslip GetPayslipById(int payslipId);
        void AddPayslip(Payslip payslip);
        void UpdatePayslip(Payslip payslip);
        void DeletePayslip(int payslipId);
        IEnumerable<Payslip> GetPayslipsByEmployeeId(int employeeId);
        IEnumerable<Payslip> GetPayslipsByDepartmentId(int departmentId);
        IEnumerable<Payslip> GetPayslipsByYearAndMonth(int year, string month);        
        bool PayslipExists(int payslipId);
    }
}
