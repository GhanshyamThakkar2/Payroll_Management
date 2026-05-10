using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // e.g., "Admin", "Employee"

        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
