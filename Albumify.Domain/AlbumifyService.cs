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

        public async Task<Album> AddAsync(string id)
        {
            // TODO: catch thrown errors here?
            // What if thirdPartMusicSerivce returns null or null object pattern
            _logger.LogInformation("Use Case: Add album with 3rd party Id of {0}", id);
            var albumToAdd = await _thirdPartyMusicService.GetAlbumAsync(id);
            return await _myCollectionRepo.AddAsync(albumToAdd);
        }
    }

    public interface I3rdPartyMusicService
    {
        Task<Album> GetAlbumAsync(string spotifyAlbumId);
    }

    // TODO: repo needs to implement
    public interface IMyCollectionRepository
    {
        Task<Album> AddAsync(Album album);
    }
}
