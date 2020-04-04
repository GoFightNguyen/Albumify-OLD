using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Albumify.Web.Models;
using Albumify.Domain.Spotify;
using Albumify.Domain;
using Albumify.Domain.Models;

namespace Albumify.Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpotifyService _spotifyMusicSource;
        private readonly MongoDbAlbumRepository _albumRepository;


        public AlbumController(ILogger<HomeController> logger, ISpotifyService spotifyMusicSource, MongoDbAlbumRepository albumRepository)
        {
            _logger = logger;
            _spotifyMusicSource = spotifyMusicSource;
            _albumRepository = albumRepository;
        }

        public async Task<IActionResult> Index(string id)
        {
            var album = await _spotifyMusicSource.GetAlbumAsync(id);
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
                NumberOfSongs = viewModel.NumberOfSongs,
                ReleaseDate = viewModel.ReleaseDate,
                SpotifyId = viewModel.SpotifyId,
                Type = viewModel.Type
            };
            await _albumRepository.AddAsync(album);

            //return View("Index", viewModel);

            return RedirectToAction("Index", "Album", new { id = viewModel.SpotifyId });
        }
    }
}
