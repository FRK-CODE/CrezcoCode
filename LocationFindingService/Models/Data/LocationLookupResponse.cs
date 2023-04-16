using IPinfo.Models;

namespace LocationFindingService.Models.Data
{
    public class LocationLookupResponse : IPResponse
    {
        public new string? City { get; set; }

        public new string? Country { get; set; }

        public new string? Latitude { get; set; }

        public new string? Longitude { get; set; }
    }
}
