using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Albumify.Web.Models
{
    public class AlbumViewModel
    {
        public string ThirdPartyId { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public string Type { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesAltText => $"Album art for {Name}";

        public AlbumViewModel(Album album)
        {
            ThirdPartyId = album.ThirdPartyId;
            Name = album.Name;
            ReleaseDate = album.ReleaseDate;
            Type = album.Type;

            Images = album.Images == null ?
                new List<ImageViewModel>() :
                album.Images.Select(a => new ImageViewModel(a)).ToList();
        }
    }
}