namespace Albumify.Domain.Models
{
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
}
