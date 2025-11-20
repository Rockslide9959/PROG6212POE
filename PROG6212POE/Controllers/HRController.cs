using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;

namespace PROG6212POE.Controllers
{
    public class HRController : Controller
    {
        public IActionResult Index()
        {
            // Display all users in table
            return View(UserStore.Users);
        }

        public IActionResult Create()
        {
            return View(new HRCreateUser());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HRCreateUser model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (UserStore.Users.ContainsKey(model.Username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(model);
            }

            // Add new user
            UserStore.Users.Add(model.Username, (model.Password, model.Role));

            TempData["SuccessMessage"] = "User created successfully!";
            return RedirectToAction("Index");
        }
    }
}
