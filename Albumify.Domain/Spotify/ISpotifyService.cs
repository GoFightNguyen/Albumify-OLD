using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Domain.Spotify
{
    public interface ISpotifyService
    {
        Task<IEnumerable<SpotifySimplifiedAlbumObject>> FindAlbumsByArtistAsync(string artistName);
    }
}
