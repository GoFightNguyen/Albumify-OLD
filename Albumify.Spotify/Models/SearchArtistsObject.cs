using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SearchArtistsObject
    {
        [JsonPropertyName("artists")]
        public PagingObject<ArtistObject> Artists { get; set; }
    }
}