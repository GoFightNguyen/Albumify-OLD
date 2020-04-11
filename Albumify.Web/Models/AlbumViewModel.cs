using Albumify.Spotify.Models;
using System.Collections.Generic;
using System.Linq;

namespace Albumify.Web.Models
{
    public class AlbumViewModel
    {
        public string SpotifyId { get; set; }

        public string Name { get; set; }

        public string ReleaseDate { get; set; }

        public int NumberOfSongs { get; set; }

        public string Type { get; set; }

        public List<ImageViewModel> Images { get; set; }

        public string ImagesAltText => $"Album art for {Name}";

        public AlbumViewModel(SpotifySimplifiedAlbumObject album)
        {
            SpotifyId = album.Id;
            Name = album.Name;
            ReleaseDate = album.ReleaseDate;
            NumberOfSongs = album.NumberOfSongs;
            Type = album.Type;

            Images = album.Images == null ? 
                new List<ImageViewModel>() : 
                album.Images.Select(a => new ImageViewModel(a)).ToList();
        }
    }
}