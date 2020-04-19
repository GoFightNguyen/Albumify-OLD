using System.Collections.Generic;

namespace Albumify.Domain.Models
{
    public class Artist
    {
        public string ThirdPartyId { get; set; }
        public string Name { get; set; }
        public List<Image> Images { get; set; } = new List<Image>();

        internal static Artist CreateForUnknown()
            => new Artist
            {
                Name = "Unknown Artist",
                ThirdPartyId = "Unknown"
            };
    }
}
