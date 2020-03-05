using System.Text.Json.Serialization;

namespace Albumify.Domain.Spotify
{
    public class SpotifyAuthenticationError
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string Description { get; set; }
    }
}
