using Albumify.Domain.Models;
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
    }
}
