using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using PROG6212POE.Services;

namespace PROG6212POE.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ClaimTableService _tablestorage;
        private readonly ClaimFileService _fileservice;

        public ClaimController(ClaimTableService tablestorage, ClaimFileService fileservice)
        {
            _tablestorage = tablestorage;
            _fileservice = fileservice;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _tablestorage.GetAllClaimsAsync();
            var files = await _fileservice.ListFilesAsync();
            return View(claims);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Claim model)
        {
            if (ModelState.IsValid)
            {
                await _tablestorage.InsertClaimAsync(model);
                return RedirectToAction("Index" , "Lecturer");
            }
            return View(model);
        }

        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _fileservice.UploadFileAsync(file);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Download(string fileName)
        {
            var stream = await _fileservice.DownloadFileAsync(fileName);
            return File(stream, "application/octet-stream", fileName);
        }
    }
}
