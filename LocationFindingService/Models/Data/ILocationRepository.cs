namespace LocationFindingService.Models.Data
{
    /// <summary>
    /// Repository for location data
    /// </summary>
    public interface ILocationRepository
    {
        /// <summary>
        /// Adds the given IP address given that that it isn't already present in the table.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="token"></param>
        /// <returns>True if the IP address was added, and False if it is already present and violates the unique constraint</returns>
        public Task<bool> AddIpAddress(string ipAddress, CancellationToken token);
    }
}
