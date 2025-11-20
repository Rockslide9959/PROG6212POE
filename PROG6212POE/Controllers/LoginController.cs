using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212POE.Models;
using System.Linq;
using System.Text.Json;

namespace PROG6212POE.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new Login());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 🔹 Authenticate user by USERNAME + PASSWORD
            var user = _context.Users
                .FirstOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password
                );

            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                return View(model);
            }

            // 🔹 Store session values for the logged-in user
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role);

            // 🔹 Redirect according to role (requirement for Part 3)
            switch (user.Role)
            {
                case "HR":
                    return RedirectToAction("Dashboard", "HR");

                case "Lecturer":
                    return RedirectToAction("Index", "Lecturer");

                case "Coordinator":
                    return RedirectToAction("PendingClaims", "Coordinator");

                case "Manager":
                    return RedirectToAction("PendingClaims", "Manager");

                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
