using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Domain
{
    public interface I3rdPartyMusicService
    {
        /// <summary>
        /// Get a specific album given its unique Third-Party Id.
        /// If the album is not found, then return <see cref="Album.CreateForUnknown(thirdpartyId)"/>.
        /// </summary>
        /// <param name="thirdPartyId"></param>
        /// <returns></returns>
        Task<Album> GetAlbumAsync(string thirdPartyId);

        /// <summary>
        /// Search artists by name.
        /// If there are no matches, then return an empty collection.
        /// </summary>
        /// <param name="name">For multiword artist names, match the words in order. For example, "Bob Dylan" will only match on anything containg "Bob Dylan".</param>
        /// <returns></returns>
        Task<List<Artist>> SearchArtistsByNameAsync(string name);

        /// <summary>
        /// Get an artist's ablums.
        /// </summary>
        /// <param name="thirdPartyId">The third party id of the artist.</param>
        /// <returns></returns>
        Task<List<Album>> GetAnArtistsAlbumsAsync(string thirdPartyId);
    }
}
