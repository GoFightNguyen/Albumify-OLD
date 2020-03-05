using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Albumify.Web.Models;
using Albumify.Domain.Spotify;

namespace Albumify.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISpotifyMusicSource _spotifyMusicSource;

        public HomeController(ILogger<HomeController> logger, ISpotifyMusicSource spotifyMusicSource)
        {
            _logger = logger;
            _spotifyMusicSource = spotifyMusicSource;
        }

        public async Task<IActionResult> Index()
        {
            var spotifyAuthenticationResult = await _spotifyMusicSource.AuthenticateUsingClientCredentialsFlowAsync();

            return View(spotifyAuthenticationResult);
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
