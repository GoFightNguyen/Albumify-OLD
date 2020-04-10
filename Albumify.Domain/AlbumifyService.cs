using Albumify.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Albumify.Domain
{
    public class AlbumifyService
    {
        private readonly ILogger<AlbumifyService> _logger;
        private readonly I3rdPartyMusicService _thirdPartyMusicService;
        private readonly IMyCollectionRepository _myCollectionRepo;

        public AlbumifyService(ILogger<AlbumifyService> logger, I3rdPartyMusicService thirdPartyMusicService, IMyCollectionRepository myCollectionRepository)
        {
            _logger = logger;
            _thirdPartyMusicService = thirdPartyMusicService;
            _myCollectionRepo = myCollectionRepository;
        }

        public async Task<Album> GetAsync(string thirdPartyid)
            => await _thirdPartyMusicService.GetAlbumAsync(thirdPartyid);

        public async Task<Album> AddAsync(string thirdPartyId)
        {
            _logger.LogInformation("Use Case - Add: 3rd party Id of {0}", thirdPartyId);

            var album = await _myCollectionRepo.FindBy3rdPartyId(thirdPartyId);
            if (!album.IsUnknown) return album;

            // TODO: acceptance/integration tests
            // TODO: catch thrown errors here?

            album = await _thirdPartyMusicService.GetAlbumAsync(thirdPartyId);
            if(album.IsUnknown)
            {
                _logger.LogWarning("Use Case - Add: The 3rd party music service was unable to find an album with Id of {0}", thirdPartyId);
                return album;
            }

            return await _myCollectionRepo.AddAsync(album);
        }
    }

    public interface I3rdPartyMusicService
    {
        /// <summary>
        /// Get a specific album given its unique Third-Party Id.
        /// If the album is not found, then <see cref="Album.CreateForUnknown(thirdpartyId)"/> is returned.
        /// </summary>
        /// <param name="thirdPartyId"></param>
        /// <returns></returns>
        Task<Album> GetAlbumAsync(string thirdPartyId);
    }

    public interface IMyCollectionRepository
    {
        Task<Album> AddAsync(Album album);
        /// <summary>
        /// Get a specific album given its unique Third-Party Id.
        /// If the album is not found, then <see cref="Album.CreateForUnknown(thirdpartyId)"/> is returned.
        /// </summary>
        /// <param name="thirdPartyId"></param>
        /// <returns></returns>
        Task<Album> FindBy3rdPartyId(string thirdPartyId);
    }
}
