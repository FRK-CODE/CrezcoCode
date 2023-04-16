using LanguageExt.Common;
using LocationFindingService.Models.Data;
using LocationFindingService.Models.Mappings;
using LocationFindingService.Models.Response;
using LocationFindingService.Models.Services;

namespace LocationFindingService.Services
{
    /// <summary>
    /// Implementation of the <see cref="ILocationService"/>
    /// </summary>
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> _logger;

        private readonly LocationLookupResponseToLocationMapper _mapper;
        private readonly ILocationLookupProvider _locationLookupProvider;
        private readonly ILocationRepository _addressRepository;

        public LocationService(ILocationLookupProvider ipLocationLookupProvider, 
            ILocationRepository addressRepository,
            ILogger<LocationService> logger)
        {
            _mapper = new LocationLookupResponseToLocationMapper();
            _locationLookupProvider = ipLocationLookupProvider;
            _addressRepository = addressRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Result<LocationResponse>> GetLocationByIPAddress(string ipAddress, CancellationToken token)
        {
            LocationResponse response;

            try
            {
                Task<LocationLookupResponse> locationLookupTask = _locationLookupProvider.GetLocationByIPAddress(ipAddress, token);
                Task<bool> addIPAddressToDbTask = _addressRepository.AddIpAddress(ipAddress, token);

                await Task.WhenAll(locationLookupTask, addIPAddressToDbTask);
                response = _mapper.LocationLookupResponseToLocation(locationLookupTask.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred in {nameof(GetLocationByIPAddress)}");
                return new Result<LocationResponse>(ex);
            }

            return response;
        }
    }
}
