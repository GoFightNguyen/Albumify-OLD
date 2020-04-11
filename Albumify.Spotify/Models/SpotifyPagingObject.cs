using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SpotifyPagingObject<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }
    }
}