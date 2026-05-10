using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Employee_Management.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Employee_Management.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _context;

        public AccountController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public async Task<IActionResult> Login()
        {
            // Ensure the database and tables are created
            _context.Database.EnsureCreated();

            // Seed a default admin if no users exist
            if (!await _context.Users.AnyAsync())
            {
                var admin = new User
                {
                    Username = "admin",
                    Password = "admin123", // Default password
                    Role = "Admin"
                };
                _context.Users.Add(admin);
                await _context.SaveChangesAsync();
            }

            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View();
            }

            var user = await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Set Session
                HttpContext.Session.SetString("UserId", user.UserId.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("EmployeeId", user.EmployeeId?.ToString() ?? "");
                HttpContext.Session.SetString("EmployeeName", user.Employee?.Name ?? "Admin");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
