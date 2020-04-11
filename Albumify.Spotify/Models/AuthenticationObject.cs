using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public struct AuthenticationObject
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(AccessToken);
    }
}
