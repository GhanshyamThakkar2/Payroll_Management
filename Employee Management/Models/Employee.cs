using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Father's Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Father's Name must contain only letters.")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Local Address is required.")]
        public string LocalAddress { get; set; }
        [Required(ErrorMessage = "Permanent Address is required.")]
        public string PermanentAddress { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        // Foreign Key for Designation (Newly Added)
        public int DesignationId { get; set; }
        public Designation Designation { get; set; }  // Navigation Property
        public bool Status { get; set; } // Active or Inactive
        [Required(ErrorMessage = "Basic salary is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public decimal BasicSalary { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Allowance { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Deduction { get; set; }
        //One to many
        public ICollection<Payslip> Payslips { get; set; }
        //public Login Login { get; set; }
    }
}
