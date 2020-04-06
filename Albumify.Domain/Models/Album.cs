using System;
using System.Collections.Generic;

namespace Albumify.Domain.Models
{
    public class Album
    {
        public const string UnknownAlbumId = "Unknown-Album-Id";

        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ReleaseDate { get; set; }
        public string Type { get; set; }
        public string ThirdPartyId { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Image> Images { get; set; } = new List<Image>();
        public List<Track> Tracks { get; set; } = new List<Track>();

        internal static Album CreateForUnknown(string thirdPartyId)
            => new Album
            {
                Artists = new List<Artist> { Artist.CreateForUnknown() },
                Id = UnknownAlbumId,
                Images = new List<Image>(),
                Label = "",
                Name = "Unknown Album",
                ReleaseDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
                ThirdPartyId = thirdPartyId,
                Tracks = new List<Track>(),
                Type = "album"
            };
    }

    public class Artist
    {
        public string ThirdPartyId { get; set; }
        public string Name { get; set; }

        internal static Artist CreateForUnknown()
            => new Artist
            {
                Name = "Unknown Artist",
                ThirdPartyId = "Unknown"
            };
    }

    public class Image
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public string Url { get; set; }
    }

    public class Track
    {
        public string Name { get; set; }
        public int Number { get; set; }
    }
}
