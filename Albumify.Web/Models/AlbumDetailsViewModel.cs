using Albumify.Domain.Spotify;
using System.Collections.Generic;
using System.Linq;

namespace Albumify.Web.Models
{
    public class AlbumDetailsViewModel
    {
        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public int NumberOfSongs { get; set; }

        public string Type { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesAltText => $"Album art for {Name}";

        public List<TrackViewModel> Tracks { get; set; }

        public AlbumDetailsViewModel(SpotifyAlbumObject album)
        {
            SpotifyId = album.Id;
            Name = album.Name;
            ReleaseDate = album.ReleaseDate.Substring(0, 4);    // should there by a SpotifyDate object?
            Type = album.Type;

            Images = album.Images == null ?
                new List<ImageViewModel>() :
                album.Images.Select(a => new ImageViewModel(a)).ToList();

            // should the tracks object take care of all these checks?
            NumberOfSongs = album.Tracks?.Items?.Count() ?? 0;

            Tracks = album.Tracks?.Items == null ?
                new List<TrackViewModel>() :
                album.Tracks.Items.Select(t => new TrackViewModel(t)).OrderBy(t => t.Number).ToList();
        }
    }
}