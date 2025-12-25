using System.Diagnostics;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibraryContext _context;

        public HomeController(ILogger<HomeController> logger, LibraryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard statistics
            var totalBooks = await _context.Books.CountAsync();
            var totalMembers = await _context.Members.CountAsync();
            var totalIssued = await _context.IssueRecords.Where(i => i.ReturnDate == null).CountAsync();
            var availableBooks = await _context.Books.Where(b => b.Quantity > 0).CountAsync();
            var overdueBooks = await _context.IssueRecords
                .Where(i => i.ReturnDate == null && i.DueDate < DateTime.Now)
                .CountAsync();

            // Recent books
            var recentBooks = await _context.Books
                .OrderByDescending(b => b.BookId)
                .Take(5)
                .ToListAsync();

            // Recent members
            var recentMembers = await _context.Members
                .OrderByDescending(m => m.MemberId)
                .Take(5)
                .ToListAsync();

            // Recent issue records
            var recentIssues = await _context.IssueRecords
                .Include(i => i.Book)
                .Include(i => i.Member)
                .OrderByDescending(i => i.IssueDate)
                .Take(5)
                .ToListAsync();

            // Calculate pending fines
            var overdueRecords = await _context.IssueRecords
                .Where(i => i.ReturnDate == null && i.DueDate < DateTime.Now)
                .ToListAsync();

            decimal totalPendingFines = 0;
            foreach (var record in overdueRecords)
            {
                var daysLate = (DateTime.Now - record.DueDate).Days;
                totalPendingFines += daysLate * 5; // Rs. 5 per day
            }

            ViewBag.TotalBooks = totalBooks;
            ViewBag.TotalMembers = totalMembers;
            ViewBag.TotalIssued = totalIssued;
            ViewBag.AvailableBooks = availableBooks;
            ViewBag.OverdueBooks = overdueBooks;
            ViewBag.TotalPendingFines = totalPendingFines;
            ViewBag.RecentBooks = recentBooks;
            ViewBag.RecentMembers = recentMembers;
            ViewBag.RecentIssues = recentIssues;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
