using Albumify.Spotify.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Spotify
{
    // TODO: remove by making functionality part of the domain interface
    public interface ISpotifyService
    {
        Task<IEnumerable<SpotifySimplifiedAlbumObject>> FindAlbumsByArtistAsync(string artistName);
    }
}
