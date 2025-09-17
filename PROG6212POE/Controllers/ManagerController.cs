using Microsoft.AspNetCore.Mvc;
using PROG6212POE.Services;

namespace PROG6212POE.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ClaimTableService _tablestorage;
        private readonly ClaimFileService _fileservice;

        public ManagerController(ClaimTableService tablestorage, ClaimFileService fileservice)
        {
            _tablestorage = tablestorage;
            _fileservice = fileservice;
        }
        public async Task<IActionResult> Index()
        {
            var claims = await _tablestorage.GetAllClaimsAsync();
            return View(claims);
        }
    }
}
