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
            if (HttpContext.Session.GetString("UserType") == "Admin")
            {
                return RedirectToAction("Index", "Station");
            }

            if (HttpContext.Session.GetString("UserType") == "Local" ||
                HttpContext.Session.GetString("UserType") == "Tourist")
            {
                return RedirectToAction("UserHome");
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

            if (user.UserType == UserType.Admin)
            {
                return RedirectToAction("Index", "Station");
            }

            return RedirectToAction("UserHome");
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
            return RedirectToAction("Login");
        }
    }
}