using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Management.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace Employee_Management.Controllers
{
    public class PayslipsController : Controller
    {
        private readonly AppDBContext _context;

        public PayslipsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Payslips
        public async Task<IActionResult> Index()
        {
            var payslips = await _context.Payslips.Include(p => p.Employee).ToListAsync();
            return View(payslips);
        }

        // GET: Payslips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payslip = await _context.Payslips
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayslipId == id);

            if (payslip == null) return NotFound();

            return View(payslip);
        }

        // GET: Payslips/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name");
            return View();
        }

        // POST: Payslips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayslipId,EmployeeId,Year,Month,PayslipDate,Status,Comments")] Payslip payslip)
        {
            if (ModelState.IsValid)
            {
                // ✅ Fetch Employee Salary Details
                var employee = await _context.Employees.FindAsync(payslip.EmployeeId);
                if (employee == null)
                {
                    ModelState.AddModelError("", "Employee not found.");
                    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", payslip.EmployeeId);
                    return View(payslip);
                }

                // ✅ Auto-fill Salary Details from Employee Table
                payslip.BasicSalary = employee.BasicSalary;
                payslip.TotalAllowances = employee.Allowance;
                payslip.TotalDeductions = employee.Deduction;

                // ✅ Auto-calculate Net Salary
                payslip.NetSalary = payslip.BasicSalary + payslip.TotalAllowances - payslip.TotalDeductions;

                _context.Add(payslip);
                await _context.SaveChangesAsync();
                GeneratePayslipPDF(payslip);
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", payslip.EmployeeId);
            return View(payslip);
        }

        // GET: Payslips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip == null) return NotFound();

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", payslip.EmployeeId);
            return View(payslip);
        }

        // POST: Payslips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayslipId,EmployeeId,Year,Month,PayslipDate,BasicSalary,TotalAllowances,TotalDeductions,Status,Comments")] Payslip payslip)
        {
            if (id != payslip.PayslipId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Prevent duplicate payslip for same Employee, Year, and Month
                    bool payslipExists = await _context.Payslips
                        .AnyAsync(p => p.EmployeeId == payslip.EmployeeId && p.Year == payslip.Year && p.Month == payslip.Month && p.PayslipId != payslip.PayslipId);

                    if (payslipExists)
                    {
                        ModelState.AddModelError("", "A payslip for this employee in the selected month and year already exists.");
                        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", payslip.EmployeeId);
                        return View(payslip);
                    }

                    // ✅ Auto-calculate Net Salary
                    payslip.NetSalary = payslip.BasicSalary + payslip.TotalAllowances - payslip.TotalDeductions;

                    _context.Update(payslip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayslipExists(payslip.PayslipId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", payslip.EmployeeId);
            return View(payslip);
        }

        // GET: Payslips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payslip = await _context.Payslips
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayslipId == id);

            if (payslip == null) return NotFound();

            return View(payslip);
        }

        // POST: Payslips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip == null) return NotFound();

            _context.Payslips.Remove(payslip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayslipExists(int id)
        {
            return _context.Payslips.Any(e => e.PayslipId == id);
        }
        [HttpGet]
        [HttpGet]
        public JsonResult GetEmployeeSalaryDetails(int employeeId)
        {
            var employee = _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new
                {
                    basicSalary = e.BasicSalary,
                    totalAllowances = e.Allowance,
                    totalDeductions = e.Deduction
                })
                .FirstOrDefault();

            if (employee == null)
            {
                return Json(new { basicSalary = 0, totalAllowances = 0, totalDeductions = 0 });
            }

            return Json(employee);
        }
        private void GeneratePayslipPDF(Payslip payslip)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "payslips");

            // Ensure directory exists
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileName = $"Payslip_{payslip.EmployeeId}_{payslip.Month}_{payslip.Year}.pdf";
            string fullPath = Path.Combine(filePath, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4);
                PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Add Title
                pdfDoc.Add(new Paragraph("Payslip"));
                pdfDoc.Add(new Paragraph(" "));

                // Add Payslip Details
                pdfDoc.Add(new Paragraph($"Employee ID: {payslip.EmployeeId}"));
                pdfDoc.Add(new Paragraph($"Year: {payslip.Year}"));
                pdfDoc.Add(new Paragraph($"Month: {payslip.Month}"));
                pdfDoc.Add(new Paragraph($"Basic Salary: {payslip.BasicSalary}"));
                pdfDoc.Add(new Paragraph($"Allowances: {payslip.TotalAllowances}"));
                pdfDoc.Add(new Paragraph($"Deductions: {payslip.TotalDeductions}"));
                pdfDoc.Add(new Paragraph($"Net Salary: {payslip.NetSalary}"));

                pdfDoc.Close();
            }
        }


    }
}
