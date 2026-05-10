using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Employee_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Employee_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;

        public HomeController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // If Admin, show main dashboard stats (can be implemented later)
            // If Employee, we could redirect to MyPayslips or show a summary
            if (HttpContext.Session.GetString("UserRole") == "Employee")
            {
                return RedirectToAction("MyPayslips");
            }

            return View();
        }

        public async Task<IActionResult> MyPayslips()
        {
            if (HttpContext.Session.GetString("Username") == null) return RedirectToAction("Login", "Account");

            string empIdStr = HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrEmpty(empIdStr)) return NotFound("Employee profile not linked to this user.");

            int employeeId = int.Parse(empIdStr);
            var payslips = await _context.Payslips
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .ToListAsync();

            return View(payslips);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
