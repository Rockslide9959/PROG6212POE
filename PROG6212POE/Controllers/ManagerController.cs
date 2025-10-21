using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using PROG6212POE.Services;

namespace PROG6212POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ClaimJsonService _claimService;
        private readonly ClaimFileService _fileService;

        public ManagerController(ClaimJsonService claimService, ClaimFileService fileService)
        {
            _claimService = claimService;
            _fileService = fileService;
        }

        public IActionResult Index()
        {
            var claims = _claimService.GetAllClaims();
            return View(claims);
        }

        [HttpPost]
        public IActionResult Approve(string claimId)
        {
            try
            {
                var claims = _claimService.GetAllClaims();
                var claim = claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim == null)
                    return NotFound();

                claim.Status = "Approved";
                _claimService.SaveAll(claims);

                TempData["SuccessMessage"] = $"Claim {claimId} approved.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult Reject(string claimId)
        {
            try
            {
                var claims = _claimService.GetAllClaims();
                var claim = claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim == null)
                    return NotFound();

                claim.Status = "Rejected";
                _claimService.SaveAll(claims);

                TempData["SuccessMessage"] = $"Claim {claimId} rejected.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Download(string fileName)
        {
            try
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
                var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream", fileName);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "File not found.";
                return RedirectToAction("Index");
            }
        }
    }
}
