using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SpotifySimplifiedAlbumObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("total_tracks")]
        public int NumberOfSongs { get; set; }

        [JsonPropertyName("album_type")]
        public string Type { get; set; }

        [JsonPropertyName("images")]
        public List<SpotifyImageObject> Images { get; set; }
    }
}