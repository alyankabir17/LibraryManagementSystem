using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class IssueRecordsController : Controller
    {
        private readonly LibraryContext _context;
        private const decimal FinePerDay = 5.00m;
        private const int MaxBooksPerMember = 5;

        public IssueRecordsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: IssueRecords
        public async Task<IActionResult> Index()
        {
            var issueRecords = await _context.IssueRecords
                .Include(i => i.Book)
                .Include(i => i.Member)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();

            // Calculate fines for overdue books
            foreach (var record in issueRecords.Where(r => r.ReturnDate == null))
            {
                record.FineAmount = CalculateFine(record.DueDate, DateTime.Now);
            }

            return View(issueRecords);
        }

        // GET: IssueRecords/Issue
        public IActionResult Issue()
        {
            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.Quantity > 0), "BookId", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name");
            
            var model = new IssueRecord
            {
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14)
            };
            
            return View(model);
        }

        // POST: IssueRecords/Issue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Issue([Bind("BookId,MemberId,IssueDate,DueDate")] IssueRecord issueRecord)
        {
            if (ModelState.IsValid)
            {
                // Check member's current issued books count
                var currentIssuedBooksCount = await _context.IssueRecords
                    .Where(i => i.MemberId == issueRecord.MemberId && i.ReturnDate == null)
                    .CountAsync();

                if (currentIssuedBooksCount >= MaxBooksPerMember)
                {
                    TempData["ErrorMessage"] = $"Cannot issue book! Member has already issued {currentIssuedBooksCount} books. Maximum limit is {MaxBooksPerMember} books at a time.";
                    ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.Quantity > 0), "BookId", "Title", issueRecord.BookId);
                    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", issueRecord.MemberId);
                    return View(issueRecord);
                }

                var book = await _context.Books.FindAsync(issueRecord.BookId);
                
                if (book == null)
                {
                    ModelState.AddModelError("", "Book not found.");
                }
                else if (book.Quantity <= 0)
                {
                    TempData["ErrorMessage"] = "Book is not available. Quantity is zero.";
                    ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.Quantity > 0), "BookId", "Title", issueRecord.BookId);
                    ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", issueRecord.MemberId);
                    return View(issueRecord);
                }
                else
                {
                    // Set due date if not provided (14 days from issue date)
                    if (issueRecord.DueDate == default)
                    {
                        issueRecord.DueDate = issueRecord.IssueDate.AddDays(14);
                    }

                    // Decrease book quantity
                    book.Quantity--;
                    _context.Update(book);
                    
                    // Create issue record
                    issueRecord.FineAmount = 0;
                    issueRecord.IsFinePaid = false;
                    _context.Add(issueRecord);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = $"Book issued successfully! Due date: {issueRecord.DueDate:dd/MM/yyyy}. Member has {currentIssuedBooksCount + 1}/{MaxBooksPerMember} books issued.";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.Quantity > 0), "BookId", "Title", issueRecord.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", issueRecord.MemberId);
            return View(issueRecord);
        }

        // GET: IssueRecords/SearchMember
        public IActionResult SearchMember()
        {
            return View();
        }

        // POST: IssueRecords/SearchMember
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchMember(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                TempData["ErrorMessage"] = "Please enter a Member ID or University ID to search.";
                return View();
            }

            // Try to parse as MemberId first
            Member? member = null;
            
            if (int.TryParse(searchTerm, out int memberId))
            {
                member = await _context.Members
                    .Include(m => m.IssueRecords)
                    .ThenInclude(i => i.Book)
                    .FirstOrDefaultAsync(m => m.MemberId == memberId);
            }

            // If not found by MemberId, search by UniversityId
            if (member == null)
            {
                member = await _context.Members
                    .Include(m => m.IssueRecords)
                    .ThenInclude(i => i.Book)
                    .FirstOrDefaultAsync(m => m.UniversityId == searchTerm);
            }

            if (member == null)
            {
                TempData["ErrorMessage"] = "Member not found with the provided ID.";
                return View();
            }

            // Get currently issued books (not returned)
            var issuedBooks = member.IssueRecords?
                .Where(i => i.ReturnDate == null)
                .OrderByDescending(i => i.IssueDate)
                .ToList() ?? new List<IssueRecord>();

            // Calculate fines
            foreach (var record in issuedBooks)
            {
                record.FineAmount = CalculateFine(record.DueDate, DateTime.Now);
            }

            ViewBag.Member = member;
            ViewBag.IssuedBooksCount = issuedBooks.Count;
            
            return View("MemberIssuedBooks", issuedBooks);
        }

        // GET: IssueRecords/Return/5
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issueRecord = await _context.IssueRecords
                .Include(i => i.Book)
                .Include(i => i.Member)
                .FirstOrDefaultAsync(m => m.IssueRecordId == id);
            
            if (issueRecord == null)
            {
                return NotFound();
            }

            if (issueRecord.ReturnDate != null)
            {
                TempData["ErrorMessage"] = "This book has already been returned.";
                return RedirectToAction(nameof(Index));
            }

            // Calculate fine if overdue
            var currentFine = CalculateFine(issueRecord.DueDate, DateTime.Now);
            ViewBag.CalculatedFine = currentFine;
            ViewBag.IsOverdue = DateTime.Now > issueRecord.DueDate;
            ViewBag.DaysOverdue = (DateTime.Now - issueRecord.DueDate).Days;

            return View(issueRecord);
        }

        // POST: IssueRecords/Return/5
        [HttpPost, ActionName("Return")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int id, DateTime returnDate, bool finePaid = false)
        {
            var issueRecord = await _context.IssueRecords
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i => i.IssueRecordId == id);

            if (issueRecord == null)
            {
                return NotFound();
            }

            if (issueRecord.ReturnDate != null)
            {
                TempData["ErrorMessage"] = "This book has already been returned.";
                return RedirectToAction(nameof(Index));
            }

            // Update return date
            issueRecord.ReturnDate = returnDate;
            
            // Calculate fine
            issueRecord.FineAmount = CalculateFine(issueRecord.DueDate, returnDate);
            issueRecord.IsFinePaid = issueRecord.FineAmount > 0 ? finePaid : true;

            _context.Update(issueRecord);

            // Increase book quantity
            if (issueRecord.Book != null)
            {
                issueRecord.Book.Quantity++;
                _context.Update(issueRecord.Book);
            }

            await _context.SaveChangesAsync();
            
            if (issueRecord.FineAmount > 0)
            {
                TempData["SuccessMessage"] = $"Book returned successfully! Fine: Rs. {issueRecord.FineAmount:F2}. Payment status: {(finePaid ? "Paid" : "Pending")}";
            }
            else
            {
                TempData["SuccessMessage"] = "Book returned successfully on time!";
            }
            
            return RedirectToAction(nameof(SearchMember));
        }

        // GET: IssueRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issueRecord = await _context.IssueRecords
                .Include(i => i.Book)
                .Include(i => i.Member)
                .FirstOrDefaultAsync(m => m.IssueRecordId == id);
            
            if (issueRecord == null)
            {
                return NotFound();
            }

            // Calculate current fine if not returned
            if (issueRecord.ReturnDate == null)
            {
                ViewBag.CurrentFine = CalculateFine(issueRecord.DueDate, DateTime.Now);
                ViewBag.IsOverdue = DateTime.Now > issueRecord.DueDate;
            }

            return View(issueRecord);
        }

        // Helper method to calculate fine
        private decimal CalculateFine(DateTime dueDate, DateTime returnDate)
        {
            if (returnDate <= dueDate)
            {
                return 0;
            }

            var daysLate = (returnDate - dueDate).Days;
            return daysLate * FinePerDay;
        }
    }
}
