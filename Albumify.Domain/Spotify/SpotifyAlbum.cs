using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;
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

        public static implicit operator Image(SpotifyImageObject spotifyImage)
            => new Image
            {
                Height = spotifyImage.Height,
                Url = spotifyImage.Url,
                Width = spotifyImage.Width
            };
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

        public static explicit operator Album(SpotifyAlbumObject spotifyAlbum)
        {
            var artists = spotifyAlbum.Artists.ConvertAll(a => (Artist)a).ToList();
            var images = spotifyAlbum.Images.ConvertAll(i => (Image)i).ToList();
            var tracks = spotifyAlbum.Tracks.Items.ConvertAll(t => (Track)t).ToList();
            return new Album
            {
                Artists = artists,
                Images = images,
                Label = spotifyAlbum.Label,
                Name = spotifyAlbum.Name,
                ReleaseDate = spotifyAlbum.ReleaseDate,
                ThirdPartyId = spotifyAlbum.Id,
                Tracks = tracks,
                Type = spotifyAlbum.Type
            };
        }
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

        public static explicit operator Track(SpotifySimplifiedTrackObject spotifyTrack)
            => new Track { Name = spotifyTrack.Name, Number = spotifyTrack.Number };
    }

    public class SpotifyArtistObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public static explicit operator Artist(SpotifyArtistObject spotifyArtist)
            => new Artist { Name = spotifyArtist.Name, ThirdPartyId = spotifyArtist.Id };
    }
}