using Albumify.Domain.Models;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SpotifyImageObject
    {
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        public static implicit operator Image(SpotifyImageObject spotifyImage)
            => new Image
            {
                Height = spotifyImage.Height,
                Url = spotifyImage.Url,
                Width = spotifyImage.Width
            };
    }
}