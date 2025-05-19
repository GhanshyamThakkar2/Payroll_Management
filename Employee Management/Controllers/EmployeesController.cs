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
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();


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
                    _context.Employees.Update(employee);
                    await _context.SaveChangesAsync();
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
        //public async Task<IActionResult> DeleteAsync(int? id)
        //{
        //    if (id == null) return NotFound();

        //    var employee = await _context.Employees
        //        .Include(e => e.Department)
        //        .Include(e => e.Designation)
        //        .FirstOrDefaultAsync(m => m.EmployeeId == id);
        //    if (employee == null) return NotFound();

        //    return View(employee);
        //}

        // ✅ POST: Employees/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(int id)
        //{
        //    var employee = _employeeRepository.GetEmployeeById(id);
        //    if (employee == null) return NotFound();

        //    if (employee.Payslips.Any())
        //    {
        //        TempData["Error"] = "Cannot delete an employee with existing payslips.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    _employeeRepository.DeleteEmployee(id);
        //    return RedirectToAction(nameof(Index));
        //}


        // ✅ AJAX: Get Designations by Department
        [HttpGet]
        public JsonResult GetDesignationsByDepartment(int departmentId)
        {
            var designations = _employeeRepository.GetDesignationsByDepartment(departmentId);
            return Json(designations);
        }
    }
}
