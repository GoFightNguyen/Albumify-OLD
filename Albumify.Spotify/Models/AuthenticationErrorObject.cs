using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class AuthenticationErrorObject
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string Description { get; set; }
    }
}
