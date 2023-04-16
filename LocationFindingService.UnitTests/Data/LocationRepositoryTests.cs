using LocationFindingService.Data;
using LocationFindingService.Models.Data;
using Moq.EntityFrameworkCore;

namespace LocationFindingService.UnitTests.Data
{
    [TestFixture]
    public class LocationRepositoryTests
    {
        private Mock<DatabaseContext> _dbContextMock;
        private ILocationRepository _locationRepository;

        [SetUp]
        public void SetUp()
        {
            _dbContextMock = new Mock<DatabaseContext>();
            _locationRepository = new LocationRepository(_dbContextMock.Object);        
        }

        [Test]
        public async Task AddIpAddress_IPAddressDoesntAlreadyExist_AddsIPAddressToDatabase()
        {
            // Arrange
            string ipAddress = "192.168.1.1";
            var token = new CancellationToken();

            var ipAddresses = new List<IPAddress>();
            _dbContextMock.Setup(c => c.IPAddresses).ReturnsDbSet(ipAddresses);

            // Act
            bool result = await _locationRepository.AddIpAddress(ipAddress, token);

            // Assert
            Assert.That(result, Is.True);
            _dbContextMock.Verify(c => c.AddAsync(It.Is<IPAddress>(ip => ip.IpAddress == ipAddress), token), Times.Once);
            _dbContextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }

        [Test]
        public async Task AddIpAddress_DuplicateIPAddressIsAdded_ReturnFalse()
        {
            // Arrange
            IPAddress ipAddress = new IPAddress("192.168.1.1");
            var token = new CancellationToken();

            var ipAddresses = new List<IPAddress> { ipAddress };
            
            _dbContextMock.Setup(c => c.IPAddresses).ReturnsDbSet(ipAddresses);

            // Act
            bool result = await _locationRepository.AddIpAddress(ipAddress.IpAddress, token);

            // Assert
            Assert.That(result, Is.False);
            _dbContextMock.Verify(c => c.AddAsync(It.Is<IPAddress>(ip => ip.IpAddress == ipAddress.IpAddress), token), Times.Never);
            _dbContextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
        }

        [Test]
        public void AddIpAddress_GenericExceptionOccurs_ThrowException()
        {
            // Arrange
            string ipAddress = "192.168.1.1";
            var token = new CancellationToken();

            var ipAddresses = new List<IPAddress>();
            _dbContextMock.Setup(c => c.IPAddresses).ReturnsDbSet(ipAddresses);

            var exception = new Exception("TestException");            

            _dbContextMock
                .Setup(c => c.AddAsync(It.Is<IPAddress>(ip => ip.IpAddress == ipAddress), token))
                .ThrowsAsync(exception);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => _locationRepository.AddIpAddress(ipAddress, token));
        }
    }
}
