using Albumify.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Albumify.Web.Models
{
    public class AlbumDetailsViewModel
    {
        // TODO: rename to 3rdPartyId
        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public ArtistViewModel Artist { get; set; }

        public string ReleaseDate { get; set; }

        public int NumberOfSongs { get; set; }

        public string Type { get; set; }

        public string Label { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesAltText => $"Album art for {Name}";

        public List<TrackViewModel> Tracks { get; set; }

        public AlbumDetailsViewModel(Album album)
        {
            SpotifyId = album.ThirdPartyId;
            Label = album.Label;
            Name = album.Name;
            ReleaseDate = album.ReleaseDate.Substring(0, 4);    // should there by a SpotifyDate object?
            Type = album.Type;

            Artist = new ArtistViewModel(album.Artists[0]);

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