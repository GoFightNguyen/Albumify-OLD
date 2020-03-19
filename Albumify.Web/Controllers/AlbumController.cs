using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Albumify.Web.Models;
using Albumify.Domain.Spotify;

namespace Albumify.Web.Controllers
{
    public class AlbumController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpotifyService _spotifyMusicSource;

        public AlbumController(ILogger<HomeController> logger, ISpotifyService spotifyMusicSource)
        {
            _logger = logger;
            _spotifyMusicSource = spotifyMusicSource;
        }

        public async Task<IActionResult> Index(string id)
        {
            var album = await _spotifyMusicSource.GetAlbumAsync(id);
            var viewModel = new AlbumDetailsViewModel(album);
            return View(viewModel);
        }
    }
}
