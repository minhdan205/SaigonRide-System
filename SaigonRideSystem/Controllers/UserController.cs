using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;
using SaigonRideSystem.Services;

namespace SaigonRideSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: User
        public IActionResult Index()
        {
            return View();
        }

        // GET: User/AdminAccounts
        public async Task<IActionResult> AdminAccounts()
        {
            var admins = await _context.Users
                .Where(u => u.UserType == UserType.Admin)
                .ToListAsync();

            return View(admins);
        }

        // GET: User/UserAccounts
        public async Task<IActionResult> UserAccounts()
        {
            var users = await _context.Users
                .Where(u => u.UserType == UserType.Local || u.UserType == UserType.Tourist)
                .ToListAsync();

            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id, string returnAction = "Index")
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.ReturnAction = returnAction;

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            bool duplicateEmail = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (duplicateEmail)
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (ModelState.IsValid)
            {
                user.PasswordHash = PasswordHelper.HashPassword(user.PasswordHash);

                _context.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User created successfully.";
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id, string returnAction = "Index")
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            LoadDropdowns();
            ViewBag.ReturnAction = returnAction;
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, string returnAction = "Index", string confirmPassword = "")
        {
            if (HttpContext.Session.GetString("UserType") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != user.UserId)
            {
                return NotFound();
            }

            ModelState.Remove("PasswordHash");

            int? currentUserId = HttpContext.Session.GetInt32("UserId");

            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                ModelState.AddModelError("", "Please enter your password to confirm this update.");
            }
            else
            {
                var currentUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == currentUserId.Value);

                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                string hashedConfirmPassword = PasswordHelper.HashPassword(confirmPassword);

                if (currentUser.PasswordHash != hashedConfirmPassword)
                {
                    ModelState.AddModelError("", "Incorrect password. User information was not updated.");
                }
            }

            bool duplicateEmail = await _context.Users
                .AnyAsync(u => u.Email == user.Email && u.UserId != user.UserId);

            if (duplicateEmail)
            {
                ModelState.AddModelError("Email", "Email already exists.");
            }

            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Country = user.Country;
                existingUser.UserType = user.UserType;
                existingUser.Passport = user.Passport;

                // Important: do not update PasswordHash here
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully.";
                return RedirectToAction(returnAction);
            }

            LoadDropdowns();
            ViewBag.ReturnAction = returnAction;
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id, string returnAction = "Index")
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.ReturnAction = returnAction;
            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnAction = "Index")
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            bool hasRentalRecords = await _context.Rentals
                .AnyAsync(r => r.UserId == id);

            if (hasRentalRecords)
            {
                TempData["ErrorMessage"] = "This user cannot be deleted because rental records are linked to this account.";
                return RedirectToAction(returnAction);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction(returnAction);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        private void LoadDropdowns()
        {
            ViewBag.UserTypes = new SelectList(Enum.GetValues(typeof(UserType)));
        }

    }
}