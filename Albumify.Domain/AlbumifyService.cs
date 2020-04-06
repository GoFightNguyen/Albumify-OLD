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

            // TODO: acceptance/integration tests
            // TODO: catch thrown errors here?
            var albumToAdd = await _thirdPartyMusicService.GetAlbumAsync(thirdPartyId);

            if(albumToAdd.Id == Album.UnknownAlbumId)
            {
                _logger.LogWarning("Use Case - Add: The 3rd party music service was unable to find an album with Id of {0}", thirdPartyId);
                return albumToAdd;
            }

            return await _myCollectionRepo.AddAsync(albumToAdd);
        }
    }

    public interface I3rdPartyMusicService
    {
        /// <summary>
        /// Get a specific album given its unique Spotify Id. 
        /// If the album is not found, then <see cref="Album.CreateForUnknown(spotifyAlbumId)"/> is returned.
        /// </summary>
        /// <param name="spotifyAlbumId"></param>
        /// <returns></returns>
        Task<Album> GetAlbumAsync(string spotifyAlbumId);
    }

    public interface IMyCollectionRepository
    {
        Task<Album> AddAsync(Album album);
    }
}
