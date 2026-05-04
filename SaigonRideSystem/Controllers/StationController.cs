using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;

namespace SaigonRideSystem.Controllers
{
    public class StationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Station
        public async Task<IActionResult> Index()
        {
            var stations = await _context.Stations
                .Include(s => s.Vehicles)
                .ToListAsync();

            return View(stations);
        }

        // GET: Station/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .Include(s => s.Vehicles)
                .FirstOrDefaultAsync(s => s.StationId == id);

            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // GET: Station/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Station/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Station station)
        {
            bool duplicateName = await _context.Stations
                .AnyAsync(s => s.StationName == station.StationName);

            if (duplicateName)
            {
                ModelState.AddModelError("StationName", "Station name already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(station);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Station created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(station);
        }

        // GET: Station/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations.FindAsync(id);

            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // POST: Station/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Station station)
        {
            if (id != station.StationId)
            {
                return NotFound();
            }

            bool duplicateName = await _context.Stations
                .AnyAsync(s => s.StationName == station.StationName && s.StationId != station.StationId);

            if (duplicateName)
            {
                ModelState.AddModelError("StationName", "Station name already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(station);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Station updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StationExists(station.StationId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(station);
        }

        // GET: Station/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .FirstOrDefaultAsync(s => s.StationId == id);

            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // POST: Station/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
            {
                return NotFound();
            }

            bool hasVehicles = await _context.Vehicles
                .AnyAsync(v => v.StationId == id);

            bool hasActiveRentals = await _context.Rentals
                .AnyAsync(r =>
                    (r.StartStationId == id || r.ReturnStationId == id)
                    && r.Status == RentalStatus.Active);

            if (hasVehicles || hasActiveRentals)
            {
                TempData["ErrorMessage"] = "This station cannot be deleted because it still contains vehicles or active rental records.";
                return RedirectToAction(nameof(Index));
            }

            _context.Stations.Remove(station);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Station deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool StationExists(int id)
        {
            return _context.Stations.Any(e => e.StationId == id);
        }
    }
}