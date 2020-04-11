using Albumify.Domain.Models;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
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