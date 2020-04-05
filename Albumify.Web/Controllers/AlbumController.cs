using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Albumify.Web.Models;
using Albumify.Domain;

namespace Albumify.Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AlbumifyService _albumifyService;

        public AlbumController(ILogger<HomeController> logger, AlbumifyService albumifyService)
        {
            _logger = logger;
            _albumifyService = albumifyService;
        }

        public async Task<IActionResult> Index(string id)
        {
            var album = await _albumifyService.GetAsync(id);
            var viewModel = new AlbumDetailsViewModel(album);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AlbumDetailsViewModel viewModel)
        {
            await _albumifyService.AddAsync(viewModel.ThirdPartyId);
            return RedirectToAction("Index", "Album", new { id = viewModel.ThirdPartyId });
        }
    }
}
