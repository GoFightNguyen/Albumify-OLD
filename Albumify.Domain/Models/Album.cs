using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Albumify.Domain.Models
{
    public class Album
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string ReleaseDate { get; set; }
        public string Type { get; set; }
        public int NumberOfSongs { get; set; }
        public string SpotifyId { get; set; }
    }
}
