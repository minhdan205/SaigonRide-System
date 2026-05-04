using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;

namespace SaigonRideSystem.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Report
        public IActionResult Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // GET: /Report/Revenue
        public async Task<IActionResult> Revenue()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var payments = await _context.Payments
                .Include(p => p.Rental)
                    .ThenInclude(r => r!.Vehicle)
                .Where(p => p.PaymentStatus == PaymentStatus.Paid)
                .ToListAsync();

            var report = payments
                .Where(p => p.Rental?.Vehicle != null)
                .GroupBy(p => p.Rental!.Vehicle!.Category)
                .Select(g => new RevenueReportItem
                {
                    VehicleCategory = g.Key.ToString(),
                    TotalTransactions = g.Count(),
                    TotalRevenue = g.Sum(p => p.Amount)
                })
                .ToList();

            return View(report);
        }

        // GET: /Report/StationInventory
        public async Task<IActionResult> StationInventory()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var stations = await _context.Stations.ToListAsync();

            var report = stations.Select(s => new StationInventoryReportItem
            {
                StationId = s.StationId,
                StationName = s.StationName,
                Location = s.Location,
                Capacity = s.Capacity,
                CurrentInventory = s.CurrentInventory,
                UtilizationRate = s.Capacity > 0
                    ? Math.Round((decimal)s.CurrentInventory / s.Capacity * 100, 2)
                    : 0,
                IsLowInventory = s.Capacity > 0 && s.CurrentInventory < s.Capacity * 0.20m
            }).ToList();

            return View(report);
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserType") == "Admin";
        }
    }
}