namespace Employee_Management.Models
{
    public class Payslip
    {
        public int PayslipId { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public string Month { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetSalary { get; set; }
        public string PaymentMethod { get; set; } // e.g., "Cash", "Bank", "Other"
        public string Status { get; set; } // e.g., "Paid", "Unpaid"
        public string Comments { get; set; }
    }

}
