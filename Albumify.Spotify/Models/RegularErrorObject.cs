using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class UnsuccessfulResponse
    {
        [JsonPropertyName("error")]
        public RegularErrorObject Error { get; set; }
    }

    public class RegularErrorObject
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
