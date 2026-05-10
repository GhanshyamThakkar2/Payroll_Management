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
using Microsoft.AspNetCore.Hosting;

namespace Employee_Management.Controllers
{
    public class PayslipsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;

        public PayslipsController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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

                // ✅ Check if payslip already exists for this period
                bool exists = await _context.Payslips.AnyAsync(p => p.EmployeeId == payslip.EmployeeId && p.Month == payslip.Month && p.Year == payslip.Year);
                if (exists)
                {
                    ModelState.AddModelError("", "A payslip for this employee and period already exists.");
                    ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Name", payslip.EmployeeId);
                    return View(payslip);
                }

                // ✅ Prevent current or future month generation
                DateTime now = DateTime.Now;
                if (payslip.Year > now.Year || (payslip.Year == now.Year && payslip.Month >= now.Month))
                {
                    ModelState.AddModelError("", "You can only generate payslips for past months.");
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
                
                // Assign employee for PDF details
                payslip.Employee = employee;
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

            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip == null) return NotFound();

            _context.Payslips.Remove(payslip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        // GET: Payslips/DownloadPdf/5
        public async Task<IActionResult> DownloadPdf(int? id)
        {
            if (id == null) return NotFound();

            var payslip = await _context.Payslips.Include(p => p.Employee).FirstOrDefaultAsync(p => p.PayslipId == id);
            if (payslip == null) return NotFound();

            string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string folderPath = Path.Combine(webRootPath, "payslips");

            // Look for the standard filename pattern
            string fileName = $"Payslip-{payslip.EmployeeId}-{payslip.Month}-{payslip.Year}.pdf";
            string fullPath = Path.Combine(folderPath, fileName);

            // If file doesn't exist, generate it with the new professional design
            if (!System.IO.File.Exists(fullPath))
            {
                GeneratePayslipPDF(payslip);
            }

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("PDF could not be generated.");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            string downloadName = $"Payslip_{payslip.Employee?.Name ?? "Employee"}_{payslip.Month}_{payslip.Year}.pdf";
            return File(fileBytes, "application/pdf", downloadName);
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
            string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(webRootPath, "payslips");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fileName = $"Payslip-{payslip.EmployeeId}-{payslip.Month}-{payslip.Year}.pdf";
            string fullPath = Path.Combine(filePath, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                Document pdfDoc = new Document(PageSize.A4, 50, 50, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Fonts
                Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, new BaseColor(65, 84, 241)); // Primary color
                Font subHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.Black);
                Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.Black);
                Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.Black);
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.Gray);

                // --- Header Section ---
                PdfPTable headerTable = new PdfPTable(1);
                headerTable.WidthPercentage = 100;
                
                PdfPCell titleCell = new PdfPCell(new Phrase("PAYROLL PRO MANAGEMENT", headerFont));
                titleCell.Border = Rectangle.NO_BORDER;
                titleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerTable.AddCell(titleCell);

                PdfPCell subTitleCell = new PdfPCell(new Phrase("Official Salary Statement", normalFont));
                subTitleCell.Border = Rectangle.NO_BORDER;
                subTitleCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerTable.AddCell(subTitleCell);

                pdfDoc.Add(headerTable);
                pdfDoc.Add(new Paragraph("\n"));
                pdfDoc.Add(new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1f, 100f, BaseColor.LightGray, Element.ALIGN_CENTER, -1f))));
                pdfDoc.Add(new Paragraph("\n"));

                // --- Employee & Period Details ---
                PdfPTable detailsTable = new PdfPTable(2);
                detailsTable.WidthPercentage = 100;
                detailsTable.SetWidths(new float[] { 1f, 1f });

                // Row 1
                detailsTable.AddCell(CreateCell("Employee Name:", boldFont, Rectangle.NO_BORDER));
                detailsTable.AddCell(CreateCell(payslip.Employee?.Name ?? "N/A", normalFont, Rectangle.NO_BORDER));

                // Row 2
                detailsTable.AddCell(CreateCell("Employee ID:", boldFont, Rectangle.NO_BORDER));
                detailsTable.AddCell(CreateCell(payslip.EmployeeId.ToString(), normalFont, Rectangle.NO_BORDER));

                // Row 3
                detailsTable.AddCell(CreateCell("Pay Period:", boldFont, Rectangle.NO_BORDER));
                detailsTable.AddCell(CreateCell($"{GetMonthName(payslip.Month)} {payslip.Year}", normalFont, Rectangle.NO_BORDER));

                // Row 4
                detailsTable.AddCell(CreateCell("Statement Date:", boldFont, Rectangle.NO_BORDER));
                detailsTable.AddCell(CreateCell(payslip.PayslipDate.ToString("MMMM dd, yyyy"), normalFont, Rectangle.NO_BORDER));

                pdfDoc.Add(detailsTable);
                pdfDoc.Add(new Paragraph("\n\n"));

                // --- Salary Table ---
                PdfPTable salaryTable = new PdfPTable(2);
                salaryTable.WidthPercentage = 100;
                salaryTable.SetWidths(new float[] { 3f, 1f });

                // Table Header
                salaryTable.AddCell(CreateCell("Description", boldFont, Rectangle.BOTTOM_BORDER, BaseColor.White, 10, true));
                salaryTable.AddCell(CreateCell("Amount", boldFont, Rectangle.BOTTOM_BORDER, BaseColor.White, 10, true));

                // Basic Salary
                salaryTable.AddCell(CreateCell("Basic Salary", normalFont, Rectangle.NO_BORDER, BaseColor.White, 8));
                salaryTable.AddCell(CreateCell($"${payslip.BasicSalary:N2}", normalFont, Rectangle.NO_BORDER, BaseColor.White, 8, false, Element.ALIGN_RIGHT));

                // Allowances
                salaryTable.AddCell(CreateCell("Total Allowances (+)", normalFont, Rectangle.NO_BORDER, new BaseColor(240, 255, 240), 8));
                salaryTable.AddCell(CreateCell($"+ ${payslip.TotalAllowances:N2}", normalFont, Rectangle.NO_BORDER, new BaseColor(240, 255, 240), 8, false, Element.ALIGN_RIGHT));

                // Deductions
                salaryTable.AddCell(CreateCell("Total Deductions (-)", normalFont, Rectangle.NO_BORDER, new BaseColor(255, 240, 240), 8));
                salaryTable.AddCell(CreateCell($"- ${payslip.TotalDeductions:N2}", normalFont, Rectangle.NO_BORDER, new BaseColor(255, 240, 240), 8, false, Element.ALIGN_RIGHT));

                // Total Net Salary
                PdfPCell totalLabelCell = CreateCell("NET SALARY", boldFont, Rectangle.TOP_BORDER, BaseColor.White, 12);
                totalLabelCell.BorderWidthTop = 1.5f;
                salaryTable.AddCell(totalLabelCell);

                PdfPCell totalValCell = CreateCell($"${payslip.NetSalary:N2}", boldFont, Rectangle.TOP_BORDER, BaseColor.White, 12, false, Element.ALIGN_RIGHT);
                totalValCell.BorderWidthTop = 1.5f;
                salaryTable.AddCell(totalValCell);

                pdfDoc.Add(salaryTable);

                // --- Footer Info ---
                pdfDoc.Add(new Paragraph("\n\n"));
                PdfPTable statusTable = new PdfPTable(1);
                statusTable.WidthPercentage = 100;
                
                PdfPCell statusCell = CreateCell($"Payment Status: {payslip.Status}", boldFont, Rectangle.BOX, new BaseColor(245, 245, 245), 10);
                statusCell.Padding = 10;
                statusCell.HorizontalAlignment = Element.ALIGN_CENTER;
                statusTable.AddCell(statusCell);
                
                pdfDoc.Add(statusTable);

                if (!string.IsNullOrEmpty(payslip.Comments))
                {
                    pdfDoc.Add(new Paragraph("\nNotes:", boldFont));
                    pdfDoc.Add(new Paragraph(payslip.Comments, normalFont));
                }

                pdfDoc.Add(new Paragraph("\n\n\n"));
                Paragraph footer = new Paragraph("This is a computer-generated document and does not require a physical signature.", smallFont);
                footer.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(footer);

                pdfDoc.Close();
            }
        }

        private PdfPCell CreateCell(string text, Font font, int border = Rectangle.BOX, BaseColor backgroundColor = null, float padding = 5, bool isHeader = false, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Border = border;
            cell.Padding = padding;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            if (backgroundColor != null) cell.BackgroundColor = backgroundColor;
            return cell;
        }


        private string GetMonthName(int month)
        {
            return new DateTime(2000, month, 1).ToString("MMMM");
        }
    }
}
