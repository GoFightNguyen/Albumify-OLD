using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Albumify.Web.Models;
using Albumify.Domain;
using Albumify.Domain.Models;

namespace Albumify.Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AlbumifyService _albumifyService;
        private readonly IMyCollectionRepository _myCollectionRepo;

        public AlbumController(ILogger<HomeController> logger, AlbumifyService albumifyService, IMyCollectionRepository myCollectionRepo)
        {
            _logger = logger;
            _albumifyService = albumifyService;
            _myCollectionRepo = myCollectionRepo;
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
            var album = new Album
            {
                Label = viewModel.Label,
                Name = viewModel.Name,
                ReleaseDate = viewModel.ReleaseDate,
                ThirdPartyId = viewModel.ThirdPartyId,
                Type = viewModel.Type
            };
            await _myCollectionRepo.AddAsync(album);

            //return View("Index", viewModel);

            return RedirectToAction("Index", "Album", new { id = viewModel.ThirdPartyId });
        }
    }
}
