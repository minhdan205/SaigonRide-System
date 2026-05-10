using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;

namespace SaigonRideSystem.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicle
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.Station)
                .ToListAsync();

            return View(vehicles);
        }

        // GET: Vehicle/Details/VE001
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var activeRental = await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.StartStation)
                .FirstOrDefaultAsync(r =>
                    r.VehicleId == id &&
                    r.Status == RentalStatus.Active);

            ViewBag.ActiveRental = activeRental;

            return View(vehicle);
        }

        // GET: Vehicle/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            bool duplicateVehicleId = await _context.Vehicles
                .AnyAsync(v => v.VehicleId == vehicle.VehicleId);

            if (duplicateVehicleId)
            {
                ModelState.AddModelError("VehicleId", "Vehicle ID already exists.");
            }

            bool stationExists = await _context.Stations
                .AnyAsync(s => s.StationId == vehicle.StationId);

            if (!stationExists)
            {
                ModelState.AddModelError("StationId", "Selected station does not exist.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Vehicle created successfully.";
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(vehicle.StationId);
            return View(vehicle);
        }

        // GET: Vehicle/Edit/VE001
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            LoadDropdowns(vehicle.StationId);
            return View(vehicle);
        }

        // POST: Vehicle/Edit/VE001
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Vehicle vehicle)
        {
            if (id != vehicle.VehicleId)
            {
                return NotFound();
            }

            bool stationExists = await _context.Stations
                .AnyAsync(s => s.StationId == vehicle.StationId);

            if (!stationExists)
            {
                ModelState.AddModelError("StationId", "Selected station does not exist.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vehicle updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VehicleId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(vehicle.StationId);
            return View(vehicle);
        }

        // GET: Vehicle/Delete/VE001
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Station)
                .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicle/Delete/VE001
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            bool hasActiveRental = await _context.Rentals
                .AnyAsync(r => r.VehicleId == id && r.Status == RentalStatus.Active);

            if (vehicle.Status == VehicleStatus.InTransit || hasActiveRental)
            {
                TempData["ErrorMessage"] = "This vehicle cannot be deleted because it is currently rented.";
                return RedirectToAction(nameof(Index));
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vehicle deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }

        private void LoadDropdowns(int? selectedStationId = null)
        {
            ViewBag.Categories = new SelectList(Enum.GetValues(typeof(VehicleCategory)));
            ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(VehicleStatus)));
            ViewBag.StationId = new SelectList(_context.Stations, "StationId", "StationName", selectedStationId);
        }
    }
}