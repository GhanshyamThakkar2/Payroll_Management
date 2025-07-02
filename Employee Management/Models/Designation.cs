using System.ComponentModel.DataAnnotations;

namespace Employee_Management.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }

        [Required(ErrorMessage = "Designation title is required.")]
        [StringLength(100, ErrorMessage = "Title can't exceed 100 characters.")]
        public string Title { get; set; }  // Example: Manager, Team Lead, Developer

        // Foreign Key to Department
        [Required(ErrorMessage = "Please select a department.")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
    }
}
