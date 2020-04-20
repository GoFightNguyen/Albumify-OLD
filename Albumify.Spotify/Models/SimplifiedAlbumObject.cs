using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SimplifiedAlbumObject
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
        public List<ImageObject> Images { get; set; }

        [JsonPropertyName("artists")]
        public List<SimplifiedArtistObject> Artists { get; set; }

        public static explicit operator Album(SimplifiedAlbumObject spotifyAlbum)
        {
            var artists = spotifyAlbum.Artists.ConvertAll(a => (Artist)a).ToList();
            var images = spotifyAlbum.Images.ConvertAll(i => (Image)i).ToList();
            return new Album
            {
                Artists = artists,
                Images = images,
                Name = spotifyAlbum.Name,
                ReleaseDate = spotifyAlbum.ReleaseDate,
                ThirdPartyId = spotifyAlbum.Id,
                Type = spotifyAlbum.Type
            };
        }
    }
}