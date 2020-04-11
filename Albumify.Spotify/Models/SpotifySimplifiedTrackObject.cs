using Albumify.Domain.Models;
using System.Text.Json.Serialization;

namespace Albumify.Spotify.Models
{
    public class SpotifySimplifiedTrackObject
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("track_number")]
        public int Number { get; set; }

        public static explicit operator Track(SpotifySimplifiedTrackObject spotifyTrack)
            => new Track { Name = spotifyTrack.Name, Number = spotifyTrack.Number };
    }
}