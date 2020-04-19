using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Albumify.Web.Models;
using Albumify.Spotify;
using System.Linq;

namespace Albumify.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpotifyService _spotifyMusicSource;

        public HomeController(ILogger<HomeController> logger, ISpotifyService spotifyMusicSource)
        {
            _logger = logger;
            _spotifyMusicSource = spotifyMusicSource;
        }

        public async Task<IActionResult> Index(string artist)
        {
            // TODO: don't need this anymore, cleanup everything related
            var albums = await _spotifyMusicSource.FindAlbumsByArtistAsync(artist);
            var viewModels = albums
                .Select(a => new AlbumViewModel(a))
                .OrderByDescending(a => a.ReleaseDate)
                .ThenBy(a => a.Name)
                .ToList();

            return View(viewModels);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
