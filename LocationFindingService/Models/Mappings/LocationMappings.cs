using IPinfo.Models;
using LocationFindingService.Models.Data;
using LocationFindingService.Models.Response;
using Riok.Mapperly.Abstractions;

namespace LocationFindingService.Models.Mappings
{
    [Mapper]
    public partial class LocationLookupResponseToLocationMapper
    {
        public partial LocationResponse LocationLookupResponseToLocation(LocationLookupResponse response);
    }

    [Mapper]
    public partial class IPResponseToIPLocationLookupResponseMapper
    {
        public partial LocationLookupResponse IPResponseToIPLocationLookupResponse(IPResponse response);
    }
}
