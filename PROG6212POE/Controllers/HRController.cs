using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using System.Text.Json;

namespace PROG6212POE.Controllers
{
    public class HRController : Controller
    {
        private readonly string _jsonPath;

        public HRController(IWebHostEnvironment env)
        {
            _jsonPath = Path.Combine(env.WebRootPath, "data", "logins.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
        }

        // ---------------------- LIST USERS ----------------------
        public IActionResult Index()
        {
            var users = LoadUsers();
            return View(users);
        }

        // ---------------------- CREATE USER ----------------------
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUser(Login user)
        {
            if (!ModelState.IsValid)
                return View(user);

            var users = LoadUsers();

            // Prevent duplicate usernames
            if (users.Any(x => x.Username == user.Username))
            {
                ModelState.AddModelError("", "Username already exists.");
                return View(user);
            }

            users.Add(user);
            SaveUsers(users);

            TempData["Success"] = "User created successfully!";
            return RedirectToAction("Index");
        }

        // ---------------------- JSON LOAD/SAVE ----------------------
        private List<Login> LoadUsers()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<Login>();

            var json = System.IO.File.ReadAllText(_jsonPath);
            return string.IsNullOrWhiteSpace(json)
                ? new List<Login>()
                : JsonSerializer.Deserialize<List<Login>>(json) ?? new List<Login>();
        }

        private void SaveUsers(List<Login> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }
    }
}
