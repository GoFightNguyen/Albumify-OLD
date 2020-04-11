using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SpotifyFindAlbumsByArtistResult
    {
        [JsonPropertyName("albums")]
        public SpotifyPagingObject<SpotifySimplifiedAlbumObject> Albums { get; set; }
    }
}