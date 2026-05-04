using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;

namespace SaigonRideSystem.Controllers
{
    public class DiscountCodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscountCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var codes = await _context.DiscountCodes
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return View(codes);
        }

        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            LoadDiscountOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountCode discountCode)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            discountCode.Code = discountCode.Code.Trim().ToUpper();

            bool duplicateCode = await _context.DiscountCodes
                .AnyAsync(d => d.Code == discountCode.Code);

            if (duplicateCode)
            {
                ModelState.AddModelError("Code", "This discount code already exists.");
            }

            if (ModelState.IsValid)
            {
                discountCode.CreatedAt = DateTime.Now;

                _context.DiscountCodes.Add(discountCode);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Discount code created successfully.";
                return RedirectToAction(nameof(Index));
            }

            LoadDiscountOptions();
            return View(discountCode);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);

            if (discountCode == null)
            {
                return NotFound();
            }

            LoadDiscountOptions();
            return View(discountCode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiscountCode discountCode)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != discountCode.DiscountCodeId)
            {
                return NotFound();
            }

            discountCode.Code = discountCode.Code.Trim().ToUpper();

            bool duplicateCode = await _context.DiscountCodes
                .AnyAsync(d => d.Code == discountCode.Code && d.DiscountCodeId != discountCode.DiscountCodeId);

            if (duplicateCode)
            {
                ModelState.AddModelError("Code", "This discount code already exists.");
            }

            if (ModelState.IsValid)
            {
                var existingCode = await _context.DiscountCodes.FindAsync(id);

                if (existingCode == null)
                {
                    return NotFound();
                }

                existingCode.CodeName = discountCode.CodeName;
                existingCode.Code = discountCode.Code;
                existingCode.DiscountPercent = discountCode.DiscountPercent;
                existingCode.IsActive = discountCode.IsActive;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Discount code updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            LoadDiscountOptions();
            return View(discountCode);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);

            if (discountCode == null)
            {
                return NotFound();
            }

            return View(discountCode);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Account");
            }

            var discountCode = await _context.DiscountCodes.FindAsync(id);

            if (discountCode == null)
            {
                return NotFound();
            }

            _context.DiscountCodes.Remove(discountCode);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Discount code deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void LoadDiscountOptions()
        {
            ViewBag.DiscountOptions = new SelectList(new[]
            {
                new { Value = 30, Text = "30%" },
                new { Value = 50, Text = "50%" },
                new { Value = 70, Text = "70%" },
                new { Value = 100, Text = "100%" }
            }, "Value", "Text");
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserType") == "Admin";
        }
    }
}