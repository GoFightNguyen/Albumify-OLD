using Albumify.Domain.Spotify;

namespace Albumify.Web.Models
{
    public class ImageViewModel
    {
        public int Height { get; set; }

        public string Path { get; set; }

        public int Width { get; set; }

        public ImageViewModel() { }

        public ImageViewModel(SpotifyImageObject image)
        {
            Height = image.Height;
            Path = image.Url;
            Width = image.Width;
        }
    }
}
