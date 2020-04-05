using Albumify.Domain.Models;

namespace Albumify.Web.Models
{
    public class ImageViewModel
    {
        public int Height { get; set; }

        public string Path { get; set; }

        public int Width { get; set; }

        public ImageViewModel() { }

        public ImageViewModel(Image image)
        {
            Height = image.Height;
            Path = image.Url;
            Width = image.Width;
        }
    }
}
