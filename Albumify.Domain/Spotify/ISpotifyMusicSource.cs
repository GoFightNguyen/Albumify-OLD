using System.Collections.Generic;
using System.Threading.Tasks;

namespace Albumify.Domain.Spotify
{
    public interface ISpotifyMusicSource
    {
        Task<SpotifyAuthenticationResult> AuthenticateUsingClientCredentialsFlowAsync();
        Task<IEnumerable<SpotifySearchAlbumResult>> FindAlbumsByArtistAsync(string artistName);
    }
}
