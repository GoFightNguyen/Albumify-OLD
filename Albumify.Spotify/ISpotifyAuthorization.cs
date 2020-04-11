using System.Threading.Tasks;

namespace Albumify.Spotify
{
    public interface ISpotifyAuthorization
    {
        Task<SpotifyAuthorizationResult> RequestAsync();
    }
}
