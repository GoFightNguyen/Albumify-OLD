using System.Linq;
using System.Threading.Tasks;
using Albumify.Domain;
using Albumify.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Albumify.Web.Controllers
{
    public class ArtistController : Controller
    {
        private readonly ILogger<ArtistController> _logger;
        private readonly AlbumifyService _albumifyService;

        public ArtistController(ILogger<ArtistController> logger, AlbumifyService albumifyService)
        {
            _logger = logger;
            _albumifyService = albumifyService;
        }

        public async Task<IActionResult> Index(string artistName)
        {
            var artists = await _albumifyService.SearchArtistsByNameAsync(artistName);
            var viewModels = artists
                .Select(a => new SearchArtistViewModel(a))
                .ToList();

            return View(viewModels);
        }
    }
}