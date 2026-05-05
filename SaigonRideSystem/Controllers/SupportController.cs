using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;

namespace SaigonRideSystem.Controllers
{
    public class SupportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Admin: list all support tickets
        // User: redirect to own tickets
        public async Task<IActionResult> Index()
        {
            string? userType = HttpContext.Session.GetString("UserType");

            if (string.IsNullOrEmpty(userType))
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != "Admin")
            {
                return RedirectToAction(nameof(MyRequests));
            }

            var tickets = await _context.SupportTickets
                .Include(t => t.User)
                .Include(t => t.Rental)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // User: view own support requests
        public async Task<IActionResult> MyRequests()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userType = HttpContext.Session.GetString("UserType");

            if (userId == null || userType == "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var tickets = await _context.SupportTickets
                .Include(t => t.Rental)
                .Where(t => t.UserId == userId.Value)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tickets);
        }

        // User: create general issue report
        public IActionResult Create()
        {
            if (!IsNormalUser())
            {
                return RedirectToAction("Login", "Account");
            }

            LoadIssueTypes();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupportTicket ticket)
        {
            if (!IsNormalUser())
            {
                return RedirectToAction("Login", "Account");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ModelState.Remove("User");
            ModelState.Remove("Rental");
            ModelState.Remove("AdminResponse");

            if (ticket.IssueType == SupportIssueType.Other && string.IsNullOrWhiteSpace(ticket.Description))
            {
                ModelState.AddModelError("Description", "Please describe your issue.");
            }

            if (ticket.IssueType == SupportIssueType.TechnicalProblem && string.IsNullOrWhiteSpace(ticket.Description))
            {
                ModelState.AddModelError("Description", "Please describe the technical problem, such as broken wheel, lock issue, or brake problem.");
            }

            if (ModelState.IsValid)
            {
                ticket.UserId = userId.Value;
                ticket.Status = SupportTicketStatus.Submitted;
                ticket.CreatedAt = DateTime.Now;
                ticket.IsResponseReadByUser = false;

                _context.SupportTickets.Add(ticket);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your support request has been submitted successfully.";
                return RedirectToAction(nameof(MyRequests));
            }

            LoadIssueTypes();
            return View(ticket);
        }

        // GET: /Support/CreateFromRental?rentalId=1
        public async Task<IActionResult> CreateFromRental(int rentalId)
        {
            if (!IsNormalUser())
            {
                return RedirectToAction("Login", "Account");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rental = await _context.Rentals
                .Include(r => r.Vehicle)
                .Include(r => r.StartStation)
                .FirstOrDefaultAsync(r =>
                    r.RentalId == rentalId &&
                    r.UserId == userId.Value &&
                    r.Status == RentalStatus.Active);

            if (rental == null)
            {
                TempData["ErrorMessage"] = "Active rental not found.";
                return RedirectToAction("ActiveRental", "Rental");
            }

            LoadIssueTypes();

            ViewBag.RentalCode = string.IsNullOrWhiteSpace(rental.RentalCode)
                ? $"Rent.No{rental.RentalId:D3}"
                : rental.RentalCode;

            ViewBag.VehicleId = rental.VehicleId;
            ViewBag.StartStation = rental.StartStation?.StationName;

            var ticket = new SupportTicket
            {
                RentalId = rental.RentalId,
                VehicleId = rental.VehicleId
            };

            return View(ticket);
        }

        // POST: /Support/CreateFromRental
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromRental(SupportTicket ticket)
        {
            if (!IsNormalUser())
            {
                return RedirectToAction("Login", "Account");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ModelState.Remove("User");
            ModelState.Remove("Rental");
            ModelState.Remove("AdminResponse");

            var rental = await _context.Rentals
                .Include(r => r.StartStation)
                .FirstOrDefaultAsync(r =>
                    r.RentalId == ticket.RentalId &&
                    r.UserId == userId.Value &&
                    r.Status == RentalStatus.Active);

            if (rental == null)
            {
                TempData["ErrorMessage"] = "Active rental not found.";
                return RedirectToAction("ActiveRental", "Rental");
            }

            if ((ticket.IssueType == SupportIssueType.TrafficAccident ||
                 ticket.IssueType == SupportIssueType.TechnicalProblem) &&
                string.IsNullOrWhiteSpace(ticket.CurrentLocation))
            {
                ModelState.AddModelError("CurrentLocation", "Please provide your current location so the support team can assist you.");
            }

            if (ticket.IssueType == SupportIssueType.Other && string.IsNullOrWhiteSpace(ticket.Description))
            {
                ModelState.AddModelError("Description", "Please describe your issue.");
            }

            if (ticket.IssueType == SupportIssueType.TechnicalProblem && string.IsNullOrWhiteSpace(ticket.Description))
            {
                ModelState.AddModelError("Description", "Please describe the technical problem, such as broken wheel, lock issue, or brake problem.");
            }

            if (ModelState.IsValid)
            {
                ticket.UserId = userId.Value;
                ticket.RentalId = rental.RentalId;
                ticket.VehicleId = rental.VehicleId;
                ticket.Status = SupportTicketStatus.Submitted;
                ticket.CreatedAt = DateTime.Now;
                ticket.IsResponseReadByUser = false;

                _context.SupportTickets.Add(ticket);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your support request has been submitted successfully.";
                return RedirectToAction(nameof(MyRequests));
            }

            LoadIssueTypes();

            ViewBag.RentalCode = string.IsNullOrWhiteSpace(rental.RentalCode)
                ? $"Rent.No{rental.RentalId:D3}"
                : rental.RentalCode;

            ViewBag.VehicleId = rental.VehicleId;
            ViewBag.StartStation = rental.StartStation?.StationName;

            return View(ticket);
        }

        // Both admin and owner user can view detail
        public async Task<IActionResult> Details(int id)
        {
            string? userType = HttpContext.Session.GetString("UserType");
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (string.IsNullOrEmpty(userType) || userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = await _context.SupportTickets
                .Include(t => t.User)
                .Include(t => t.Rental)
                .FirstOrDefaultAsync(t => t.SupportTicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (userType != "Admin" && ticket.UserId != userId.Value)
            {
                return RedirectToAction("Login", "Account");
            }

            if (userType != "Admin" && !string.IsNullOrWhiteSpace(ticket.AdminResponse))
            {
                ticket.IsResponseReadByUser = true;
                await _context.SaveChangesAsync();
            }

            return View(ticket);
        }

        // Admin response
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int id, string adminResponse, SupportTicketStatus status)
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = await _context.SupportTickets.FindAsync(id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(adminResponse))
            {
                TempData["ErrorMessage"] = "Admin response cannot be empty.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ticket.AdminResponse = adminResponse.Trim();
            ticket.Status = status;
            ticket.RespondedAt = DateTime.Now;
            ticket.IsResponseReadByUser = false;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Response sent to user successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private bool IsNormalUser()
        {
            string? userType = HttpContext.Session.GetString("UserType");
            return userType == "Local" || userType == "Tourist";
        }

        private void LoadIssueTypes()
        {
            ViewBag.IssueTypes = new SelectList(Enum.GetValues(typeof(SupportIssueType)));
        }
    }
}