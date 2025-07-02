using System;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class Payslip
    {
        public int PayslipId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(2000, 2100, ErrorMessage = "Please enter a valid year.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Payslip date is required.")]
        [DataType(DataType.Date)]
        public DateTime PayslipDate { get; set; }

        [Required(ErrorMessage = "Basic Salary is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Basic Salary must be non-negative.")]
        public decimal BasicSalary { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Allowances must be non-negative.")]
        public decimal TotalAllowances { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Deductions must be non-negative.")]
        public decimal TotalDeductions { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Net Salary must be non-negative.")]
        public decimal NetSalary { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        public string Status { get; set; } // e.g., "Paid", "Unpaid"

        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters.")]
        public string Comments { get; set; }
    }
}
