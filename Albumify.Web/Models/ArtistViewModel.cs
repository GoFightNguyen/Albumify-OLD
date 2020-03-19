using Albumify.Domain.Spotify;

namespace Albumify.Web.Models
{
    public class ArtistViewModel
    {
        public string SpotifyId { get; set; }
        public string Name { get; set; }

        public ArtistViewModel() { }

        public ArtistViewModel(SpotifyArtistObject artist)
        {
            SpotifyId = artist.Id;
            Name = artist.Name;
        }
    }
}