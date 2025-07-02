using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class BankDetail
    {
        public int BankDetailId { get; set; }

        [Required(ErrorMessage = "Account holder name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string AccountHolderName { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Account number must be 9 to 18 digits.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "IFSC Code is required.")]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Enter a valid IFSC Code.")]
        public string IFSCCode { get; set; }

        [Required(ErrorMessage = "Bank name is required.")]
        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters.")]
        public string BankName { get; set; }

        [Required(ErrorMessage = "Branch name is required.")]
        [StringLength(100, ErrorMessage = "Branch name cannot exceed 100 characters.")]
        public string Branch { get; set; }

        [Required]
        public int EmployeeId { get; set; }  // Foreign Key

        public Employee Employee { get; set; }
    }
}
