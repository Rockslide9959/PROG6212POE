using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using System.Text.Json;

namespace PROG6212POE.Controllers
{
    public class CoordinatorController : Controller
    {
        private readonly string _jsonPath;
        private readonly string _uploadsFolder;
        private readonly IWebHostEnvironment _env;

        public CoordinatorController(IWebHostEnvironment env)
        {
            _env = env;
            _jsonPath = Path.Combine(env.WebRootPath, "data", "claims.json");
            _uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
            Directory.CreateDirectory(_uploadsFolder);
        }

        // ===================== COORDINATOR VIEW =====================

        // 1️⃣ View all pending claims
        [HttpGet]
        public IActionResult Index()
        {
            var claims = LoadClaims()
                .Where(c => c.Status == "Pending")
                .OrderByDescending(c => c.DateSubmitted)
                .ToList();
            return View(claims);
        }

        // 2️⃣ Verify a claim (approve to manager)
        [HttpPost]
        public IActionResult Verify(string id)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
                return NotFound("Claim not found.");

            claim.Status = "Verified by Coordinator";  // ✅ must match manager filter
            SaveClaims(claims);

            TempData["SuccessMessage"] = $"Claim {claim.ClaimId} verified successfully.";
            return RedirectToAction("Index");
        }

        // 3️⃣ Reject a claim
        [HttpPost]
        public IActionResult Reject(string id)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
                return NotFound("Claim not found.");

            claim.Status = "Rejected by Coordinator";
            SaveClaims(claims);

            TempData["ErrorMessage"] = $"Claim {claim.ClaimId} has been rejected.";
            return RedirectToAction("Index");
        }

        // 4️⃣ View the uploaded document
        [HttpGet]
        public IActionResult ViewDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return NotFound();

            var safePath = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(_env.WebRootPath, safePath);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found.");

            var contentType = GetContentType(fullPath);
            return PhysicalFile(fullPath, contentType);
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        // ===================== JSON STORAGE =====================

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
