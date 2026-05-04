using Microsoft.AspNetCore.Mvc;

namespace SaigonRideSystem.Controllers
{
    public class LandingController : Controller
    {
        // GET: /Landing/Index
        public IActionResult Index()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserType")))
            {
                return RedirectToAction("Home", "Account");
            }

            return View();
        }
    }
}