using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using System.Text.Json;

namespace PROG6212POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly string _jsonPath;
        private readonly string _uploadsFolder;
        private readonly IWebHostEnvironment _env;

        public ManagerController(IWebHostEnvironment env)
        {
            _env = env;
            _jsonPath = Path.Combine(env.WebRootPath, "data", "claims.json");
            _uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
            Directory.CreateDirectory(_uploadsFolder);
        }

        // 1️⃣ View all verified claims
        public IActionResult Index()
        {
            var claims = LoadClaims()
                .Where(c => c.Status == "Verified by Coordinator")
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();

            return View(claims);
        }

        // 2️⃣ Approve claim
        [HttpPost]
        public IActionResult Approve(string claimId)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
                return NotFound();

            claim.Status = "Approved";
            SaveClaims(claims);

            TempData["SuccessMessage"] = $"Claim {claimId} approved successfully.";
            return RedirectToAction("Index");
        }

        // 3️⃣ Reject claim
        [HttpPost]
        public IActionResult Reject(string claimId)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == claimId);
            if (claim == null)
                return NotFound();

            claim.Status = "Rejected";
            SaveClaims(claims);

            TempData["ErrorMessage"] = $"Claim {claimId} rejected.";
            return RedirectToAction("Index");
        }

        // 4️⃣ Download supporting document
        public IActionResult Download(string fileName)
        {
            var filePath = Path.Combine(_uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                TempData["ErrorMessage"] = "File not found.";
                return RedirectToAction("Index");
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/octet-stream", fileName);
        }

        // 5️⃣ JSON helpers (same as ClaimController)
        private List<Claim> LoadClaims()
        {
            if (!System.IO.File.Exists(_jsonPath))
                return new List<Claim>();

            var json = System.IO.File.ReadAllText(_jsonPath);
            return string.IsNullOrWhiteSpace(json)
                ? new List<Claim>()
                : JsonSerializer.Deserialize<List<Claim>>(json) ?? new List<Claim>();
        }

        private void SaveClaims(List<Claim> claims)
        {
            var json = JsonSerializer.Serialize(claims, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(_jsonPath, json);
        }
    }
}
