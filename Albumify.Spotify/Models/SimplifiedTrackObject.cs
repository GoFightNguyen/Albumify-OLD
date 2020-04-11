using Albumify.Domain.Models;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SimplifiedTrackObject
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("track_number")]
        public int Number { get; set; }

        public static explicit operator Track(SimplifiedTrackObject spotifyTrack)
            => new Track { Name = spotifyTrack.Name, Number = spotifyTrack.Number };
    }
}