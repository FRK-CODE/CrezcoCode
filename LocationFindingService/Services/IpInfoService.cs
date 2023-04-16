using IPinfo;
using IPinfo.Exceptions;
using IPinfo.Models;
using LocationFindingService.Models.Data;
using LocationFindingService.Models.Mappings;
using LocationFindingService.Models.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace LocationFindingService.Services
{
    /// <summary>
    /// Implementation of the <see cref="ILocationLookupProvider"/> using an IPInfoClient to perform the lookup
    /// </summary>
    public class IpInfoService : ILocationLookupProvider
    {
        private readonly ILogger<IpInfoService> _logger;
        private readonly IPinfoClient _client;
        private readonly IPResponseToIPLocationLookupResponseMapper _mapper;

        private readonly IAsyncPolicy<LocationLookupResponse> _retryPolicy =
            Policy<LocationLookupResponse>
                .Handle<RequestFailedGeneralException>()
                .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3));

        public IpInfoService(IConfiguration configuration, ILogger<IpInfoService> logger)
        {
            _mapper = new IPResponseToIPLocationLookupResponseMapper();
            _client = CreateClient(configuration);
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<LocationLookupResponse> GetLocationByIPAddress(string ipAddress, CancellationToken token)
        {
            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    IPResponse response = await _client.IPApi.GetDetailsAsync(ipAddress, token);
                    return _mapper.IPResponseToIPLocationLookupResponse(response);
                });
            }
            catch (RequestFailedGeneralException ex)
            {
                _logger.LogError(ex, "Request failed when calling IP info");
                throw;
            }
        }

        private IPinfoClient CreateClient(IConfiguration configuration)
        {
            string ipInfoToken = configuration.GetValue<string>("IPInfoToken");
            return new IPinfoClient.Builder()
                .AccessToken(ipInfoToken)
                .HttpClientConfig(config => config.Timeout(TimeSpan.FromSeconds(5)))
                .Build();
        }
    }
}
