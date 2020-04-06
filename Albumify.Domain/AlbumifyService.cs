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
            // TODO: acceptance/integration tests
            // TODO: catch thrown errors here?
            // What if thirdPartMusicSerivce returns null object pattern
            _logger.LogInformation("Use Case: Add album with 3rd party Id of {0}", thirdPartyId);
            var albumToAdd = await _thirdPartyMusicService.GetAlbumAsync(thirdPartyId);
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
