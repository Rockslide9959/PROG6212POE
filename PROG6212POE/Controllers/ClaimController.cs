using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using System.Text.Json;

namespace PROG6212POE.Controllers
{
    public class ClaimController : Controller
    {
        private readonly string _jsonPath;
        private readonly string _uploadsFolder;

        public ClaimController(IWebHostEnvironment env)
        {
            _jsonPath = Path.Combine(env.WebRootPath, "data", "claims.json");
            _uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
            Directory.CreateDirectory(_uploadsFolder);
        }

        // ✅ 1. View all claims
        public IActionResult Index()
        {
            var claims = LoadClaims();
            return View(claims.OrderByDescending(c => c.DateSubmitted));
        }

        // ✅ 2. Display the Create form
        public IActionResult Create() => View();

        // ✅ 3. Handle Claim Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim model, IFormFile? document)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // --- File Upload Validation ---
                if (document != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(document.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                        throw new InvalidOperationException("Invalid file type. Only PDF, DOCX, and XLSX files are allowed.");

                    if (document.Length > 5 * 1024 * 1024)
                        throw new InvalidOperationException("File size exceeds 5MB.");

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(_uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    model.DocumentName = document.FileName;

                    // Simple encryption simulation (requirement: secure storage)
                    model.EncryptedFilePath = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(filePath));
                }

                model.Status = "Pending";
                model.DateSubmitted = DateTime.Now;

                // --- Save to JSON ---
                var claims = LoadClaims();
                claims.Add(model);
                SaveClaims(claims);

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        // ✅ 4. Track specific claim
        [HttpGet]
        public IActionResult Track(string id)
        {
            var claim = LoadClaims().FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
                return NotFound("Claim not found.");
            return View(claim);
        }

        // ✅ 5. Coordinator verifies/rejects
        [HttpPost]
        public IActionResult Verify(string id, bool approve)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            claim.Status = approve ? "Verified by Coordinator" : "Rejected by Coordinator";
            SaveClaims(claims);

            TempData["SuccessMessage"] = $"Claim {claim.ClaimId} {(approve ? "verified" : "rejected")}.";
            return RedirectToAction("Index");
        }

        // ✅ 6. Manager approves/rejects
        [HttpPost]
        public IActionResult Approve(string id, bool approve)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim == null) return NotFound();

            claim.Status = approve ? "Approved by Manager" : "Rejected by Manager";
            SaveClaims(claims);

            TempData["SuccessMessage"] = $"Claim {claim.ClaimId} {(approve ? "approved" : "rejected")}.";
            return RedirectToAction("Index");
        }

        // ✅ 7. Helper Methods (JSON Persistence)
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
