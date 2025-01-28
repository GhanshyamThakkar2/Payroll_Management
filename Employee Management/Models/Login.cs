namespace Employee_Management.Models
{
    public class Login
    {
        public int LoginId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; } // e.g., "Admin" or "Employee"
    }

}