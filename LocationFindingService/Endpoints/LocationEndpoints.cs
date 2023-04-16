using LanguageExt.Common;
using LocationFindingService.Models.Filters;
using LocationFindingService.Models.Response;
using LocationFindingService.Models.Services;
using LocationFindingService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LocationFindingService.Endpoints
{
    /// <summary>
    /// Contains the location endpoints
    /// </summary>
    public class LocationEndpoints : IEndpointDefinition
    {
        /// <inheritdoc />
        public void DefineEndpoints(WebApplication app)
        {
            app.MapGet("/location/{ipAddress}", GetLocationByIpAddress)
            .CacheOutput(x => x.SetVaryByRouteValue("ipAddress"))
            .AddEndpointFilter<IPAddressFilter>()
            .WithOpenApi()
            .WithDescription("Finds the location relating to a given IP address")
            .WithSummary("GetLocationByIPAddress")
            .Produces<LocationResponse>()
            .Produces((int)HttpStatusCode.BadRequest)
            .Produces((int)HttpStatusCode.NotFound)
            .Produces((int)HttpStatusCode.InternalServerError);
        }

        /// <inheritdoc />
        public void DefineServices(IServiceCollection services)
        {
            services.AddTransient<ILocationService, LocationService>();
        }

        /// <summary>
        /// Gets location by IP address
        /// </summary>
        /// <param name="locationService"></param>
        /// <param name="ipAddress"></param>
        /// <returns>Returns the location response or a bad request/internal server status code depending if an exception has been returend from the service</returns>
        internal async Task<IResult> GetLocationByIpAddress([FromRoute] string ipAddress, ILocationService locationService, CancellationToken token = default)
        {
            Result<LocationResponse> result = await locationService.GetLocationByIPAddress(ipAddress, token);

            return result.Match(response =>
            {
                if (string.IsNullOrEmpty(response.Longitude) && string.IsNullOrEmpty(response.Latitude))
                {
                    return Results.NotFound();
                }

                return Results.Ok(response);
            }, exception =>
            {
                if (token.IsCancellationRequested)
                {
                    return Results.BadRequest();
                }

                return Results.StatusCode(500);
            });
        }
    }
}
