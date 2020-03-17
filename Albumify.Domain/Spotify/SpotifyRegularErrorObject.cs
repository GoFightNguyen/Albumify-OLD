using System.Text.Json.Serialization;

namespace Albumify.Domain.Spotify
{
    public class SpotifyUnsuccessfulResponse
    {
        [JsonPropertyName("error")]
        public SpotifyRegularErrorObject Error { get; set; }
    }

    public class SpotifyRegularErrorObject
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
