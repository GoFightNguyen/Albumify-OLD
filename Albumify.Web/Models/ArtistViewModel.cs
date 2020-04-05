using Albumify.Domain.Models;

namespace Albumify.Web.Models
{
    public class ArtistViewModel
    {
        // TODO: rename to ThirdPartyId
        public string SpotifyId { get; set; }
        public string Name { get; set; }

        public ArtistViewModel() { }

        public ArtistViewModel(Artist artist)
        {
            SpotifyId = artist.ThirdPartyId;
            Name = artist.Name;
        }
    }
}