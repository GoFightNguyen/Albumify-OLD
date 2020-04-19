using Albumify.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

        public async Task<Album> GetAsync(string thirdPartyId)
        {
            var album = await _myCollectionRepo.FindBy3rdPartyIdAsync(thirdPartyId);
            if (!album.IsUnknown) return album;

            album = await _thirdPartyMusicService.GetAlbumAsync(thirdPartyId);
            if (album.IsUnknown)
                _logger.LogWarning("Use Case - Get: Unknown Album: thirdPartyId {0}", thirdPartyId);

            return album;
        }

        public async Task<Album> AddAsync(string thirdPartyId)
        {
            _logger.LogInformation("Use Case - Add: thirdPartyId {0}", thirdPartyId);

            var album = await _myCollectionRepo.FindBy3rdPartyIdAsync(thirdPartyId);
            if (!album.IsUnknown) return album;

            album = await _thirdPartyMusicService.GetAlbumAsync(thirdPartyId);
            if(album.IsUnknown)
            {
                _logger.LogWarning("Use Case - Add: Unknown Album: thirdPartyId {0}", thirdPartyId);
                return album;
            }

            return await _myCollectionRepo.AddAsync(album);
        }

        /// <summary>
        /// Search artists by name.
        /// </summary>
        /// <param name="name">For multiword artist names, match the words in order. For example, "Bob Dylan" will only match on anything containg "Bob Dylan".</param>
        /// <returns></returns>
        public async Task<List<Artist>> SearchArtistsByNameAsync(string name)
        {
            // TODO: flag as in collection
            // TODO: order in collection as higher
            return await _thirdPartyMusicService.SearchArtistsByNameAsync(name);
        }
    }
}
