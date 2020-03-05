using System.Threading.Tasks;

namespace Albumify.Domain.Spotify
{
    public interface ISpotifyMusicSource
    {
        Task<SpotifyAuthenticationResult> AuthenticateUsingClientCredentialsFlowAsync();
    }
}
