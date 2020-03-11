using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Albumify.Domain.Spotify
{
    public class SpotifyImageObject
    {
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class SpotifySimplifiedAlbumObject
    {
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

    public class SpotifyPagingObject
    {
        [JsonPropertyName("items")]
        public List<SpotifySimplifiedAlbumObject> Items { get; set; }
    }

    public class SpotifyFindAlbumsByArtistResult
    {
        [JsonPropertyName("albums")]
        public SpotifyPagingObject PagingObject { get; set; }
    }
}