using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Albumify.Domain.Spotify
{
    public class SpotifySearchAlbumResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        // TODO: change to string if time is being written to storage
        [JsonPropertyName("release_date")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("total_tracks")]
        public int NumberOfSongs { get; set; }

        //album_type
    }

    public class SpotifyFindAlbumsByArtistResult
    {
        [JsonPropertyName("albums")]
        public SpotifySearchAlbumsByArtistResult Albums { get; set; }
    }

    public class SpotifySearchAlbumsByArtistResult
    {
        [JsonPropertyName("items")]
        public List<SpotifySearchAlbumResult> Items { get; set; }
    }
}