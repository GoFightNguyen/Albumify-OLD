using Albumify.Domain.Spotify;

namespace Albumify.Web.Models
{
    public class TrackViewModel
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public TrackViewModel()
        {

        }

        public TrackViewModel(SpotifySimplifiedTrackObject track)
        {
            Name = track.Name;
            Number = track.Number;
        }
    }
}