using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Management.Models;

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
            var appDBContext = _context.Payslips.Include(p => p.Employee);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Payslips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payslip = await _context.Payslips
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayslipId == id);
            if (payslip == null)
            {
                return NotFound();
            }

            return View(payslip);
        }

        // GET: Payslips/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: Payslips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PayslipId,EmployeeId,Year,Month,PayslipDate,BasicSalary,TotalAllowances,TotalDeductions,NetSalary,PaymentMethod,Status,Comments")] Payslip payslip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payslip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", payslip.EmployeeId);
            return View(payslip);
        }

        // GET: Payslips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", payslip.EmployeeId);
            return View(payslip);
        }

        // POST: Payslips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PayslipId,EmployeeId,Year,Month,PayslipDate,BasicSalary,TotalAllowances,TotalDeductions,NetSalary,PaymentMethod,Status,Comments")] Payslip payslip)
        {
            if (id != payslip.PayslipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payslip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayslipExists(payslip.PayslipId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", payslip.EmployeeId);
            return View(payslip);
        }

        // GET: Payslips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payslip = await _context.Payslips
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(m => m.PayslipId == id);
            if (payslip == null)
            {
                return NotFound();
            }

            return View(payslip);
        }

        // POST: Payslips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip != null)
            {
                _context.Payslips.Remove(payslip);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayslipExists(int id)
        {
            return _context.Payslips.Any(e => e.PayslipId == id);
        }
    }
}
