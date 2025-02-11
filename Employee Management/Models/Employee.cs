using System;

namespace Employee_Management.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string LocalAddress { get; set; }
        public string PermanentAddress { get; set; }
        //public string Nationality { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; } 
        public bool Status { get; set; } // Active or Inactive
        public decimal BasicSalary { get; set; }
        public decimal Allowance { get; set; }
        public decimal Deduction { get; set; }
        public int BankDetailId { get; set; }
        //Navif=gation Property One-To-One
        public BankDetail BankDetail { get; set; }
        public int PayslipId { get; set; }
        //One to many
        public ICollection<Payslip> Payslip { get; set; }
        //public Login Login { get; set; }
    }
}
