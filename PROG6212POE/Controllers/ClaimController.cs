using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using System.Text.Json;

namespace PROG6212POE.Controllers
{
    public class ClaimController : Controller
    {
        private readonly string _jsonPath;
        private readonly string _uploadsFolder;
        private readonly IWebHostEnvironment _env;

        public ClaimController(IWebHostEnvironment env)
        {
            _env = env;
            _jsonPath = Path.Combine(env.WebRootPath, "data", "claims.json");
            _uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

            // ✅ Ensure folders exist
            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
            Directory.CreateDirectory(_uploadsFolder);
        }

        // 1️⃣ View all claims (lecturer)
        public IActionResult Index()
        {
            var claims = LoadClaims();
            return View(claims.OrderByDescending(c => c.DateSubmitted));
        }

        // 2️⃣ Display form
        public IActionResult Create() => View();

        // 3️⃣ Handle submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim model, IFormFile? document)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // ✅ Handle document upload if provided
                if (document != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(document.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                        throw new InvalidOperationException("Invalid file type. Only PDF, DOCX, and XLSX files are allowed.");

                    if (document.Length > 5 * 1024 * 1024)
                        throw new InvalidOperationException("File size exceeds 5MB.");

                    var uniqueFileName = Guid.NewGuid() + extension;
                    var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    model.DocumentName = document.FileName;
                    model.EncryptedFilePath = $"/uploads/{uniqueFileName}";
                }

                // ✅ Initialize claim values
                model.ClaimId = Guid.NewGuid().ToString();
                model.Status = "Pending";
                model.DateSubmitted = DateTime.Now;

                // ✅ Load, add, and save claim
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

        // 4️⃣ Helper functions
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
