using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class LoginController : Controller
    {
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

            // Check if user exists
            if (!UserStore.Users.ContainsKey(model.Username))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            var (Password, Role) = UserStore.Users[model.Username];

            // Check password
            if (model.Password != Password)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(model);
            }

            // Redirect based on role
            return Role switch
            {
                "Lecturer" => RedirectToAction("Index", "Lecturer"),
                "Coordinator" => RedirectToAction("Index", "Coordinator"),
                "Manager" => RedirectToAction("Index", "Manager"),
                "HR" => RedirectToAction("Index", "HR"),
                _ => View(model)
            };
        }
    }
}
