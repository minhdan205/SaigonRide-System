using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaigonRideSystem.Data;
using SaigonRideSystem.Models;
using SaigonRideSystem.Services;

namespace SaigonRideSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToAction("Home");
            }

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string hashedPassword = PasswordHelper.HashPassword(model.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserType", user.UserType.ToString());

            return RedirectToAction("Home");
        }

        // GET: /Account/Home
        public async Task<IActionResult> Home()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);

            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: /Account/UserHome
        public IActionResult UserHome()
        {
            string? userType = HttpContext.Session.GetString("UserType");

            if (userType != "Local" && userType != "Tourist")
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Landing");
        }

        // GET: /Account/SignUp
        public IActionResult SignUp()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToAction("Home");
            }

            return View();
        }

        // POST: /Account/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool duplicateEmail = await _context.Users.AnyAsync(u => u.Email == model.Email);

            if (duplicateEmail)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            bool isVietnameseUser = SignUpViewModel.IsVietnam(model.Country);

            var user = new User
            {
                Name = model.FullName,
                Email = model.Email,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                Country = model.Country,
                UserType = isVietnameseUser ? UserType.Local : UserType.Tourist,
                Passport = isVietnameseUser ? null : model.Passport
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserType", user.UserType.ToString());

            return RedirectToAction("Home");
        }
    }
}