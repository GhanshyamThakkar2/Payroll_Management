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
    public class BankDetailsController : Controller
    {
        private readonly AppDBContext _context;

        public BankDetailsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: BankDetails
        
        public async Task<IActionResult> Index()
        {
            var bankDetails = await _context.BankDetails
                                            .Include(b => b.Employee)   // <-- Include Employee correctly
                                            .ToListAsync();             // <-- Fetch including Employee
            return View(bankDetails);
        }


        // GET: BankDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDetail = await _context.BankDetails
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.BankDetailId == id);
            if (bankDetail == null)
            {
                return NotFound();
            }

            return View(bankDetail);
        }

        // GET: BankDetails/Create
        public IActionResult Create()
        {
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "Name"); // Populating Employee Dropdown
            return View();
        }

        // POST: BankDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BankDetailId,AccountHolderName,AccountNumber,IFSCCode,BankName,Branch,EmployeeId")] BankDetail bankDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bankDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "Name", bankDetail.EmployeeId); // Keep dropdown populated
            return View(bankDetail);
        }

        // GET: BankDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDetail = await _context.BankDetails.FindAsync(id);
            if (bankDetail == null)
            {
                return NotFound();
            }
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "Name", bankDetail.EmployeeId); // Keep existing selection
            return View(bankDetail);
        }

        // POST: BankDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BankDetailId,AccountHolderName,AccountNumber,IFSCCode,BankName,Branch,EmployeeId")] BankDetail bankDetail)
        {
            if (id != bankDetail.BankDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bankDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankDetailExists(bankDetail.BankDetailId))
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
            ViewData["Employees"] = new SelectList(_context.Employees, "EmployeeId", "Name", bankDetail.EmployeeId); // Keep dropdown populated
            return View(bankDetail);
        }

        // GET: BankDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankDetail = await _context.BankDetails
                .Include(b => b.Employee)
                .FirstOrDefaultAsync(m => m.BankDetailId == id);
            if (bankDetail == null)
            {
                return NotFound();
            }

            return View(bankDetail);
        }

        // POST: BankDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankDetail = await _context.BankDetails.FindAsync(id);
            if (bankDetail != null)
            {
                _context.BankDetails.Remove(bankDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankDetailExists(int id)
        {
            return _context.BankDetails.Any(e => e.BankDetailId == id);
        }
    }
}
