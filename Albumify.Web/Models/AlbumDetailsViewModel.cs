using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Albumify.Web.Models
{
    public class AlbumDetailsViewModel
    {
        public string ThirdPartyId { get; set; }

        public string Name { get; set; }

        public List<ArtistViewModel> Artists { get; set; }

        public string ReleaseDate { get; set; }

        public int NumberOfSongs { get; set; }

        public string Type { get; set; }

        public string Label { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesAltText => $"Album art for {Name}";

        public List<TrackViewModel> Tracks { get; set; }

        public AlbumDetailsViewModel(Album album)
        {
            ThirdPartyId = album.ThirdPartyId;
            Label = album.Label;
            Name = album.Name;
            ReleaseDate = album.ReleaseDate.Substring(0, 4);    // should there by a SpotifyDate object?
            Type = album.Type;

            Artists = album.Artists.ConvertAll(a => new ArtistViewModel(a));

            Images = album.Images == null ?
                new List<ImageViewModel>() :
                album.Images.Select(a => new ImageViewModel(a)).ToList();

            NumberOfSongs = album.Tracks.Count();
            Tracks = album.Tracks.Select(t => new TrackViewModel(t)).OrderBy(t => t.Number).ToList();
        }

        public AlbumDetailsViewModel()
        {
        }
    }
}