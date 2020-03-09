using System;

namespace Albumify.Domain.Spotify
{
    public class SpotifyAuthorizationException : Exception
    {
        public SpotifyAuthorizationException(SpotifyAuthorizationError error)
            : base($"Failed to authenticate with Spotify using Client Credentials Flow: {error.Description}. " +
                    "Please verify the configuration for SpotifyClientId and SpotifyClientSecret")
        {

        }
    }
}
