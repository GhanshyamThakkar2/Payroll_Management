using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Management.Models;
using Employee_Management.Interface;

namespace Employee_Management.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IEmployee _employeeRepository;

        public EmployeesController(AppDBContext context, IEmployee employeeRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
        }

        // ✅ GET: Employees (With Pagination)
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var employees = _employeeRepository.GetAllEmployees()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return View(employees);
        }

        // ✅ GET: Employees/Details/5
        public async Task<IActionResult> DetailsAsync(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // ✅ GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            ViewData["DesignationId"] = new SelectList(new List<Designation>(), "DesignationId", "Title");
            return View();
        }


        // ✅ POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (!string.IsNullOrEmpty(employee.Username))
                {
                    bool userExists = await _context.Users.AnyAsync(u => u.Username == employee.Username);
                    if (userExists)
                    {
                        ModelState.AddModelError("Username", "This username is already taken.");
                        ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
                        ViewData["DesignationId"] = new SelectList(_context.Designations.Where(d => d.DepartmentId == employee.DepartmentId), "DesignationId", "Title", employee.DesignationId);
                        return View(employee);
                    }
                }

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Create User record for the new employee
                if (!string.IsNullOrEmpty(employee.Username) && !string.IsNullOrEmpty(employee.Password))
                {
                    var user = new User
                    {
                        Username = employee.Username,
                        Password = employee.Password, // In a real app, hash this!
                        Role = employee.Role ?? "Employee",
                        EmployeeId = employee.EmployeeId
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations.Where(d => d.DepartmentId == employee.DepartmentId), "DesignationId", "Title", employee.DesignationId);

            return View(employee);
        }

        // ✅ GET: Employees/Edit/5
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null) return NotFound();

            // Load User credentials
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == employee.EmployeeId);
            if (user != null)
            {
                employee.Username = user.Username;
                employee.Role = user.Role;
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations.Where(d => d.DepartmentId == employee.DepartmentId), "DesignationId", "Title", employee.DesignationId);

            return View(employee);
        }

        // ✅ POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(int id, Employee employee)
        {
            if (id != employee.EmployeeId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                    // Update User record
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.EmployeeId == employee.EmployeeId);
                    if (user != null)
                    {
                        user.Username = employee.Username ?? user.Username;
                        user.Role = employee.Role ?? user.Role;
                        
                        // Only update password if a new one is provided
                        if (!string.IsNullOrEmpty(employee.Password))
                        {
                            user.Password = employee.Password;
                        }
                        
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    else if (!string.IsNullOrEmpty(employee.Username))
                    {
                        // If no user exists but username is provided, create one
                        var newUser = new User
                        {
                            Username = employee.Username,
                            Password = employee.Password ?? "123456", // default password if none provided
                            Role = employee.Role ?? "Employee",
                            EmployeeId = employee.EmployeeId
                        };
                        _context.Add(newUser);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_employeeRepository.EmployeeExists(employee.EmployeeId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", employee.DepartmentId);
            ViewData["DesignationId"] = new SelectList(_context.Designations.Where(d => d.DepartmentId == employee.DepartmentId), "DesignationId", "Title", employee.DesignationId);


            return View(employee);
        }

        // ✅ GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null) return NotFound();

            // Deleting directly as requested for simple confirmation
            if (employee.Payslips != null && employee.Payslips.Any())
            {
                TempData["Error"] = "Cannot delete an employee with existing payslips.";
                return RedirectToAction(nameof(Index));
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // ✅ AJAX: Get Designations by Department
        [HttpGet]
        public JsonResult GetDesignationsByDepartment(int departmentId)
        {
            var designations = _employeeRepository.GetDesignationsByDepartment(departmentId)
                .Select(d => new { 
                    designationId = d.DesignationId, 
                    title = d.Title 
                });
            return Json(designations);
        }
    }
}
