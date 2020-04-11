using Albumify.Domain.Models;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class ArtistObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public static explicit operator Artist(ArtistObject spotifyArtist)
            => new Artist { Name = spotifyArtist.Name, ThirdPartyId = spotifyArtist.Id };
    }
}