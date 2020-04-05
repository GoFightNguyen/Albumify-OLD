using Albumify.Domain.Models;

namespace Albumify.Web.Models
{
    public class ArtistViewModel
    {
        public string ThirdPartyId { get; set; }
        public string Name { get; set; }

        public ArtistViewModel() { }

        public ArtistViewModel(Artist artist)
        {
            ThirdPartyId = artist.ThirdPartyId;
            Name = artist.Name;
        }
    }
}