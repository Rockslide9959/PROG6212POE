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
            if (ModelState.IsValid)
            {
                // No authentication logic; redirect to Home page
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}