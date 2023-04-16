using IPinfo;
using LocationFindingService.Data;
using LocationFindingService.Endpoints;
using LocationFindingService.Models.Data;
using LocationFindingService.Models.Response;
using LocationFindingService.Models.Services;
using LocationFindingService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace LocationFindingService.IntegrationTests
{
    [TestFixture]
    public class LocationServiceTests
    {
        private InMemoryDatabaseContext _dbContext;
        private LocationRepository _locationRepository;
        private LocationService _locationService;
        private LocationEndpoints _locationEndpoints;
        private ILocationLookupProvider _locationLookupProvider;

        private const string IP_INFO_CLIENT_TOKEN = "b0d17d2cd4cafe";

        [SetUp]
        public void Setup()
        {
            DbContextOptions<DatabaseContext> options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;
            _dbContext = new InMemoryDatabaseContext(options);

            _locationRepository = new LocationRepository(_dbContext);

            var config = new Dictionary<string, string>
            {
                {"IPInfoToken", IP_INFO_CLIENT_TOKEN}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(config)
                .Build();

            _locationLookupProvider = new IpInfoService(configuration, new Mock<ILogger<IpInfoService>>().Object);
            _locationService = new LocationService(_locationLookupProvider, _locationRepository, new Mock<ILogger<LocationService>>().Object); 
            _locationEndpoints = new LocationEndpoints();
        }

        [Test]
        public async Task GetLocationByIPAddress_ValidIpAddress_LocationResponse()
        {
            // Arrange
            string testIPAddress = "216.58.194.174";

            // Act
            var result = await _locationEndpoints.GetLocationByIpAddress(testIPAddress, _locationService);

            // Assert
            Assert.That(result, Is.InstanceOf<Ok<LocationResponse>>());
            var okResult = result as Ok<LocationResponse>;
            Assert.Multiple(() =>
            {
                Assert.That(okResult.Value.City, Is.EqualTo("Des Moines"));
                Assert.That(okResult.Value.Country, Is.EqualTo("US"));
                Assert.That(okResult.Value.Latitude, Is.EqualTo("41.6005"));
                Assert.That(okResult.Value.Longitude, Is.EqualTo("-93.6091"));
            });

            // Verify that the IP address was added to the database
            var ipAddressInDatabase = await _dbContext.IPAddresses.FirstOrDefaultAsync(x => x.IpAddress == testIPAddress);
            Assert.That(ipAddressInDatabase, Is.Not.Null);
        }

        [Test]
        public async Task GetLocationByIPAddress_BogonIpAddress_NotFoundResponse()
        {
            // Arrange
            string testIPAddress = "127.0.0.1";

            // Act
            var result = await _locationEndpoints.GetLocationByIpAddress(testIPAddress, _locationService);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFound>());
            var notFoundResult = result as NotFound;
            Assert.That(notFoundResult.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));

            // Verify that the IP address was added to the database
            var ipAddressInDatabase = await _dbContext.IPAddresses.FirstOrDefaultAsync(x => x.IpAddress == testIPAddress);
            Assert.That(ipAddressInDatabase, Is.Not.Null);
        }

        [Test]
        public async Task GetLocationByIPAddress_CancelledToken_BadRequestResponse()
        {
            // Arrange
            string testIPAddress = "127.0.0.1";
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            var result = await _locationEndpoints.GetLocationByIpAddress(testIPAddress, _locationService, cancellationTokenSource.Token);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequest>());
            var badRequestResult = result as BadRequest;
            Assert.That(badRequestResult.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }
    }
}