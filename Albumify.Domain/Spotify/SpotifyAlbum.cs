using System;
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

    public class SpotifyFindAlbumsByArtistResult
    {
        [JsonPropertyName("albums")]
        public SpotifyPagingObject<SpotifySimplifiedAlbumObject> Albums { get; set; }
    }

    public class SpotifyAlbumObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("images")]
        public List<SpotifyImageObject> Images { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("album_type")]
        public string Type { get; set; }

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; }

        [JsonPropertyName("tracks")]
        public SpotifyPagingObject<SpotifySimplifiedTrackObject> Tracks { get; set; }

        [JsonPropertyName("artists")]
        public List<SpotifyArtistObject> Artists { get; set; }

        internal static SpotifyAlbumObject CreateForUnknownAlbum(string id)
            => new SpotifyAlbumObject
            {
                Id = id,
                Label = "",
                Name = "Unknown Album",
                ReleaseDate = DateTime.Today.Date.ToString(),
                Artists = new List<SpotifyArtistObject>(),
                Images = new List<SpotifyImageObject>(),
                Tracks = new SpotifyPagingObject<SpotifySimplifiedTrackObject>(),
                Type = "album"
            };
    }

    public class SpotifyPagingObject<T>
    {
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }
    }

    public class SpotifySimplifiedTrackObject
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("track_number")]
        public int Number { get; set; }
    }

    public class SpotifyArtistObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}