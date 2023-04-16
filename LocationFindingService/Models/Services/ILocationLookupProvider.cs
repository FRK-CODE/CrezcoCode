using LocationFindingService.Models.Data;

namespace LocationFindingService.Models.Services
{
    /// <summary>
    /// Provides methods for performing a location lookup
    /// </summary>
    public interface ILocationLookupProvider
    {
        /// <summary>
        /// Get a location response by IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="token"></param>
        /// <returns>A location lookup response</returns>
        public Task<LocationLookupResponse> GetLocationByIPAddress(string ipAddress, CancellationToken token);
    }
}
