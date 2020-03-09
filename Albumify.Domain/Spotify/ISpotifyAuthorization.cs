using System.Threading.Tasks;

namespace Albumify.Domain.Spotify
{
    public interface ISpotifyAuthorization
    {
        Task<SpotifyAuthorizationResult> RequestAsync();
    }
}
