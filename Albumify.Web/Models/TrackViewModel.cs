using Albumify.Domain.Models;

namespace Albumify.Web.Models
{
    public class TrackViewModel
    {
        public string Name { get; set; }
        public int Number { get; set; }

        public TrackViewModel()
        {
        }

        public TrackViewModel(Track track)
        {
            Name = track.Name;
            Number = track.Number;
        }
    }
}