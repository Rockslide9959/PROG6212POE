using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class LoginController : Controller
    {
        // Hard-coded users
        private readonly Dictionary<string, (string Password, string Role)> _users =
            new Dictionary<string, (string Password, string Role)>
            {
                { "lecturer1", ("password123", "Lecturer") },
                { "coordinator1", ("coord123", "Coordinator") },
                { "manager1", ("manager123", "Manager") },
                { "admin", ("admin123", "HR") } // Super Admin HR
            };

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

            // Check if username exists
            if (!_users.ContainsKey(model.Username))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var (Password, Role) = _users[model.Username];

            // Check password
            if (model.Password != Password)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // Redirect by role
            switch (Role)
            {
                case "Lecturer":
                    return RedirectToAction("Index", "Lecturer");

                case "Coordinator":
                    return RedirectToAction("Index", "Coordinator");

                case "Manager":
                    return RedirectToAction("Index", "Manager");

                case "HR":
                    return RedirectToAction("Index", "HR");

                default:    
                    ModelState.AddModelError("", "User has an unknown role.");
                    return View(model);
            }
        }
    }
}
