using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Models;
using PROG6212POE.Services;

namespace PROG6212POE.Controllers
{
    public class ClaimController : Controller
    {
        private readonly ClaimTableService _tablestorage;

        public ClaimController(ClaimTableService tablestorage)
        {
            _tablestorage = tablestorage;
        }

        public async Task<IActionResult> Index()
        {
            var claims = await _tablestorage.GetAllClaimsAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
