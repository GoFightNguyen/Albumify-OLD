using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Albumify.Web.Models
{
    public class SearchArtistViewModel
    {
        public string ThirdPartyId { get; set; }
        public string Name { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesAltText => $"Artist image for {Name}";

        public SearchArtistViewModel(Artist artist)
        {
            ThirdPartyId = artist.ThirdPartyId;
            Name = artist.Name;
            Images = artist.Images.Select(a => new ImageViewModel(a)).ToList();
        }
    }
}