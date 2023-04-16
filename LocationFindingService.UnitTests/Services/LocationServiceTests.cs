using LanguageExt.Common;
using LocationFindingService.Models.Data;
using LocationFindingService.Models.Response;
using LocationFindingService.Models.Services;
using LocationFindingService.Services;
using Microsoft.Extensions.Logging;

namespace LocationFindingService.UnitTests.Services
{
    [TestFixture]
    public class LocationServiceTests
    {
        private Mock<ILocationLookupProvider> _mockLocationLookupProvider;
        private Mock<ILocationRepository> _addressRepositoryMock;
        private LocationService _locationService;

        [SetUp]
        public void Setup()
        {
            _mockLocationLookupProvider = new Mock<ILocationLookupProvider>();
            _addressRepositoryMock = new Mock<ILocationRepository>();
            _locationService = new LocationService(_mockLocationLookupProvider.Object, _addressRepositoryMock.Object, new Mock<ILogger<LocationService>>().Object);
        }

        [Test]
        public async Task GetLocationByIPAddress_ValidIpAddress_ReturnsLocationResponse()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var ipResponse = new LocationLookupResponse { City = "Swindon", Country = "UK" };
            var token = new CancellationToken();

            _mockLocationLookupProvider.Setup(x => x.GetLocationByIPAddress(ipAddress, token)).ReturnsAsync(ipResponse);
            _addressRepositoryMock.Setup(x => x.AddIpAddress(ipAddress, token)).ReturnsAsync(true);
        
            // Act
            Result<LocationResponse> result = await _locationService.GetLocationByIPAddress(ipAddress, token);
        
            // Assert
            Assert.That(result.IsSuccess, Is.True);
            result.IfSucc(response => {
                Assert.Multiple(() =>
                {
                    Assert.That(response.City, Is.EqualTo(ipResponse.City));
                    Assert.That(response.Country, Is.EqualTo(ipResponse.Country));
                });
            });
        }
        
        [Test]
        public async Task GetLocationByIPAddress_ThrowsException_ReturnsFailedResult()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var expectedException = new Exception("An error occurred");
            var token = new CancellationToken();

            _mockLocationLookupProvider.Setup(x => x.GetLocationByIPAddress(ipAddress, token)).ThrowsAsync(expectedException);
        
            // Act
            var result = await _locationService.GetLocationByIPAddress(ipAddress, token);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            result.IfFail(exception =>
            {
                Assert.That(exception, Is.EqualTo(expectedException));
            });                
        }
    }
}
