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

        public IActionResult Index()
        {
            var claims = LoadClaims();
            return View(claims.OrderByDescending(c => c.DateSubmitted));
        }

        public IActionResult Create() => View();

        // Prevent crashes when uploading huge files
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(10_000_000)] // 10MB request limit (change if needed)
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

                    var uniqueFileName = Guid.NewGuid() + extension;
                    var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

                    // Protect against huge file crashes
                    try
                    {
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await document.CopyToAsync(stream);
                    }
                    catch
                    {
                        throw new InvalidOperationException("The file is too large or could not be uploaded.");
                    }

                    model.DocumentName = document.FileName;             // original name
                    model.EncryptedFilePath = $"/uploads/{uniqueFileName}"; // stored file
                }

                model.ClaimId = Guid.NewGuid().ToString();
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

        // DOWNLOAD DOCUMENT WITH ORIGINAL FILE NAME
        public IActionResult Download(string id)
        {
            var claims = LoadClaims();
            var claim = claims.FirstOrDefault(c => c.ClaimId == id);

            if (claim == null || string.IsNullOrEmpty(claim.EncryptedFilePath))
                return NotFound("Document not found.");

            var filePath = Path.Combine(_env.WebRootPath, claim.EncryptedFilePath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound("Document file is missing.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            // Return file using ORIGINAL name
            return File(fileBytes, "application/octet-stream", claim.DocumentName);
        }

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
