using LanguageExt.Common;
using LocationFindingService.Endpoints;
using LocationFindingService.Models.Response;
using LocationFindingService.Models.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace LocationFindingService.UnitTests.Endpoints
{
    [TestFixture]
    public class LocationEndpointsTests
    {
        private Mock<ILocationService> _mockLocationService;
        private LocationEndpoints _locationEndpoints;

        [SetUp]
        public void Setup()
        {
            _mockLocationService = new Mock<ILocationService>();
            _locationEndpoints = new LocationEndpoints();
        }

        [Test]
        public async Task GetLocationByIpAddress_ValidIp_ReturnsOkResult()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var expectedResponse = new LocationResponse { City = "Swindon", Country = "UK", Longitude = "0.5", Latitude = "0.5" };
            _mockLocationService.Setup(x => x.GetLocationByIPAddress(ipAddress, new CancellationToken())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _locationEndpoints.GetLocationByIpAddress(ipAddress, _mockLocationService.Object);

            // Assert
            Assert.That(result, Is.InstanceOf<Ok<LocationResponse>>());
            var okResult = result as Ok<LocationResponse>;
            Assert.That(okResult.Value, Is.EqualTo(expectedResponse));
        }

        [Test]
        public async Task GetLocationByIpAddress_ServiceResultHasNullValues_ReturnsNotFoundResult()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var expectedResponse = new LocationResponse { Longitude = null, Latitude = null };
            _mockLocationService.Setup(x => x.GetLocationByIPAddress(ipAddress, new CancellationToken())).ReturnsAsync(expectedResponse);

            // Act
            var result = await _locationEndpoints.GetLocationByIpAddress(ipAddress, _mockLocationService.Object);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFound>());
            var notFoundResult = result as NotFound;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetLocationByIpAddress_ServiceThrowsException_ReturnsStatusCodeWith500Error()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var serviceResult = new Result<LocationResponse>(new Exception("An error occurred"));
            _mockLocationService.Setup(x => x.GetLocationByIPAddress(ipAddress, new CancellationToken())).ReturnsAsync(serviceResult);

            // Act
            var result = await _locationEndpoints.GetLocationByIpAddress(ipAddress, _mockLocationService.Object);

            // Assert
            Assert.That(result, Is.InstanceOf<StatusCodeHttpResult>());
            var internalServerErrorResult = result as StatusCodeHttpResult;
            Assert.That(internalServerErrorResult.StatusCode, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }
    }
}
