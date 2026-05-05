using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;
using SaigonRideSystem.Services;

namespace SaigonRideSystem.Controllers
{
    public class RentalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PricingService _pricingService;

        public RentalController(ApplicationDbContext context, PricingService pricingService)
        {
            _context = context;
            _pricingService = pricingService;
        }

        // GET: /Rental/AvailableVehicles
        public async Task<IActionResult> AvailableVehicles()
        {
            if (!IsNormalUser())
            {
                return RedirectToAction("Login", "Account");
            }

            var vehicles = await _context.Vehicles
                .Include(v => v.Station)
                .Where(v => v.Status == VehicleStatus.Available)
                .ToListAsync();

            return View(vehicles);
        }

        // POST: /Rental/Rent/VE001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(string id)
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

            bool alreadyHasActiveRental = await _context.Rentals
                .AnyAsync(r => r.UserId == userId.Value && r.Status == RentalStatus.Active);

            if (alreadyHasActiveRental)
            {
                TempData["ErrorMessage"] = "You already have an active rental. Please return it before renting another vehicle.";
                return RedirectToAction(nameof(ActiveRental));
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Vehicle not found.";
                return RedirectToAction(nameof(AvailableVehicles));
            }

            if (vehicle.Status != VehicleStatus.Available)
            {
                TempData["ErrorMessage"] = "This vehicle is not available.";
                return RedirectToAction(nameof(AvailableVehicles));
            }

            var rental = new Rental
            {
                UserId = userId.Value,
                VehicleId = vehicle.VehicleId,
                StartStationId = vehicle.StationId,
                StartTime = DateTime.Now,
                Status = RentalStatus.Active
            };

            vehicle.Status = VehicleStatus.InTransit;

            if (vehicle.Station != null && vehicle.Station.CurrentInventory > 0)
            {
                vehicle.Station.CurrentInventory -= 1;
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            rental.RentalCode = $"Rent.No{rental.RentalId:D3}";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Vehicle rented successfully. Your rental code is {rental.RentalCode}.";
            return RedirectToAction(nameof(ActiveRental));
        }

        // GET: /Rental/ActiveRental
        public async Task<IActionResult> ActiveRental()
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
                .FirstOrDefaultAsync(r => r.UserId == userId.Value && r.Status == RentalStatus.Active);

            return View(rental);
        }

        // GET: /Rental/Return/5
        public async Task<IActionResult> Return(int id)
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
                .FirstOrDefaultAsync(r => r.RentalId == id && r.UserId == userId.Value && r.Status == RentalStatus.Active);

            if (rental == null)
            {
                TempData["ErrorMessage"] = "Active rental not found.";
                return RedirectToAction(nameof(ActiveRental));
            }

            ViewBag.ReturnStationId = new SelectList(_context.Stations, "StationId", "StationName");
            return View(rental);
        }

        // POST: /Rental/ReturnConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnConfirmed(int rentalId, int returnStationId, string? discountCode)
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
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == userId.Value && r.Status == RentalStatus.Active);

            if (rental == null || rental.Vehicle == null)
            {
                TempData["ErrorMessage"] = "Active rental not found.";
                return RedirectToAction(nameof(ActiveRental));
            }

            var returnStation = await _context.Stations.FindAsync(returnStationId);

            if (returnStation == null)
            {
                TempData["ErrorMessage"] = "Return station not found.";
                return RedirectToAction(nameof(ActiveRental));
            }

            DiscountCode? appliedCode = null;

            if (!string.IsNullOrWhiteSpace(discountCode))
            {
                string normalizedCode = discountCode.Trim().ToUpper();

                appliedCode = await _context.DiscountCodes
                    .FirstOrDefaultAsync(d => d.Code == normalizedCode && d.IsActive);

                if (appliedCode == null)
                {
                    TempData["ErrorMessage"] = "Invalid or inactive discount code.";
                    return RedirectToAction(nameof(Return), new { id = rental.RentalId });
                }
            }

            DateTime endTime = DateTime.Now;

            var pricingResult = _pricingService.CalculateFare(
                rental.Vehicle.Category,
                rental.StartTime,
                endTime,
                returnStation.Capacity,
                returnStation.CurrentInventory
            );

            decimal finalFare = pricingResult.FinalFare;
            decimal codeDiscountAmount = 0m;

            if (appliedCode != null)
            {
                codeDiscountAmount = finalFare * appliedCode.DiscountPercent / 100m;
                finalFare -= codeDiscountAmount;
            }

            rental.EndTime = endTime;
            rental.ReturnStationId = returnStation.StationId;
            rental.TotalFare = finalFare;
            rental.DiscountApplied = pricingResult.DiscountApplied;
            rental.DiscountAmount = pricingResult.DiscountAmount;
            rental.Status = RentalStatus.Completed;

            if (appliedCode != null)
            {
                rental.DiscountCodeId = appliedCode.DiscountCodeId;
                rental.AppliedDiscountCode = appliedCode.Code;
                rental.CodeDiscountPercent = appliedCode.DiscountPercent;
                rental.CodeDiscountAmount = codeDiscountAmount;
            }

            rental.Vehicle.Status = VehicleStatus.Available;
            rental.Vehicle.StationId = returnStation.StationId;

            if (returnStation.CurrentInventory < returnStation.Capacity)
            {
                returnStation.CurrentInventory += 1;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vehicle returned successfully. Please complete payment.";
            return RedirectToAction(nameof(Payment), new { id = rental.RentalId });
        }

        // GET: /Rental/Payment/5
        public async Task<IActionResult> Payment(int id)
        {
            if (!IsNormalUser())
            {
                return RedirectToAction("Login", "Account");
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            string? userType = HttpContext.Session.GetString("UserType");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var rental = await _context.Rentals
                .Include(r => r.Vehicle)
                .Include(r => r.StartStation)
                .Include(r => r.ReturnStation)
                .Include(r => r.DiscountCode)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RentalId == id && r.UserId == userId.Value);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.Payment != null)
            {
                return RedirectToAction(nameof(Receipt), new { id = rental.RentalId });
            }

            List<PaymentMethod> paymentMethods = userType == "Tourist"
                ? new List<PaymentMethod> { PaymentMethod.Cash, PaymentMethod.ApplePay, PaymentMethod.PayPal }
                : new List<PaymentMethod> { PaymentMethod.Cash, PaymentMethod.MoMo, PaymentMethod.VNPay };

            ViewBag.PaymentMethods = new SelectList(paymentMethods);
            return View(rental);
        }

        // POST: /Rental/Pay
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int rentalId, PaymentMethod paymentMethod)
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
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RentalId == rentalId && r.UserId == userId.Value);

            if (rental == null)
            {
                return NotFound();
            }

            if (rental.Payment != null)
            {
                return RedirectToAction(nameof(Receipt), new { id = rental.RentalId });
            }

            var payment = new Payment
            {
                RentalId = rental.RentalId,
                PaymentMethod = paymentMethod,
                Amount = rental.TotalFare,
                PaymentDate = DateTime.Now,
                PaymentStatus = PaymentStatus.Paid
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Payment completed successfully.";
            return RedirectToAction(nameof(Receipt), new { id = rental.RentalId });
        }

        // GET: /Rental/Receipt/5
        public async Task<IActionResult> Receipt(int id)
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
                .Include(r => r.ReturnStation)
                .Include(r => r.DiscountCode)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.RentalId == id && r.UserId == userId.Value);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: /Rental/History
        public async Task<IActionResult> History()
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

            var rentals = await _context.Rentals
                .Include(r => r.Vehicle)
                .Include(r => r.StartStation)
                .Include(r => r.ReturnStation)
                .Include(r => r.Payment)
                .Include(r => r.DiscountCode)
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return View(rentals);
        }

        private bool IsNormalUser()
        {
            string? userType = HttpContext.Session.GetString("UserType");
            return userType == "Local" || userType == "Tourist";
        }
    }
}