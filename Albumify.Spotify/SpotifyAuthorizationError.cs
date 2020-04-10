using System.Text.Json.Serialization;

namespace Albumify.Spotify
{
    public class SpotifyAuthorizationError
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string Description { get; set; }
    }
}
