using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SpotifyFindAlbumsByArtistResult
    {
        [JsonPropertyName("albums")]
        public PagingObject<SimplifiedAlbumObject> Albums { get; set; }
    }
}