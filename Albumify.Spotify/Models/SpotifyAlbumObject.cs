using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
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
}