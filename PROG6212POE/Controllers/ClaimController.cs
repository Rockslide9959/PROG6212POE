using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using PROG6212POE.Services;

namespace PROG6212POE.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ClaimJsonService _claimService;
        private readonly ClaimFileService _fileService;

        public ClaimController(ClaimJsonService claimService, ClaimFileService fileService)
        {
            _claimService = claimService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var claims = _claimService.GetAllClaims();
            return View(claims);
        }

        public IActionResult Create() => View();

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
                    var fileName = await _fileService.UploadFileAsync(document);
                    model.DocumentName = fileName;
                }

                _claimService.AddClaim(model);
                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }
    }
}
