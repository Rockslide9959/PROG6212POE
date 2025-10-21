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

            Directory.CreateDirectory(Path.GetDirectoryName(_jsonPath)!);
            Directory.CreateDirectory(_uploadsFolder);
        }

        // ✅ 1. View all claims
        public IActionResult Index()
        {
            var claims = LoadClaims();
            return View(claims.OrderByDescending(c => c.DateSubmitted));
        }

        // ✅ 2. Display claim creation form
        public IActionResult Create() => View();

        // ✅ 3. Create new claim (with viewable upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim model, IFormFile? document)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                if (document != null)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(document.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                        throw new InvalidOperationException("Invalid file type. Only PDF, DOCX, and XLSX files are allowed.");

                    if (document.Length > 5 * 1024 * 1024)
                        throw new InvalidOperationException("File size exceeds 5MB.");

                    // ✅ Ensure uploads folder exists
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    // ✅ Create a unique, safe filename
                    var uniqueFileName = Guid.NewGuid() + extension;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // ✅ Save file to disk
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    // ✅ Store web-relative path (used for View)
                    model.DocumentName = document.FileName;
                    model.EncryptedFilePath = $"/uploads/{uniqueFileName}";
                }


                model.Status = "Pending";
                model.DateSubmitted = DateTime.Now;

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

        // ✅ 4. Track claim
        [HttpGet]
        public IActionResult Track(string id)
        {
            var claim = LoadClaims().FirstOrDefault(c => c.ClaimId == id);
            if (claim == null)
                return NotFound("Claim not found.");
            return View(claim);
        }

        // ✅ 5. Open/View uploaded document
        [HttpGet]
        public IActionResult ViewDocument(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return NotFound();

            var physicalPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (!System.IO.File.Exists(physicalPath))
                return NotFound("File not found.");

            var contentType = GetContentType(physicalPath);
            return PhysicalFile(physicalPath, contentType);
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

        // ✅ 6. JSON load/save
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
