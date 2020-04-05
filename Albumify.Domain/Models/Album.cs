using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Albumify.Domain.Models
{
    // TODO: separate MongoDB concern
    public class Album
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ReleaseDate { get; set; }
        public string Type { get; set; }
        public string ThirdPartyId { get; set; }
        public List<Artist> Artists { get; set; } = new List<Artist>();
        public List<Image> Images { get; set; } = new List<Image>();
        public List<Track> Tracks { get; set; } = new List<Track>();

        public int NumberOfSongs => Tracks.Count;
    }

    public class Artist
    {
        public string ThirdPartyId { get; set; }
        public string Name { get; set; }
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
