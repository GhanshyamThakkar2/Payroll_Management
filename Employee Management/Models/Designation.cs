namespace Employee_Management.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }
        public string Title { get; set; }  // Example: Manager, Team Lead, Developer
        // Foreign Key to Department
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
