using LanguageExt.Common;
using LocationFindingService.Models.Response;

namespace LocationFindingService.Models.Services
{
    /// <summary>
    /// Provides methods for getting location by some data (IP address)
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Returns the location based on a given IP address
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="token"></param>
        /// <returns>A result containing a location response if successful, a result containing an exception otherwise</returns>
        public Task<Result<LocationResponse>> GetLocationByIPAddress(string ipAddress, CancellationToken token);
    }
}
