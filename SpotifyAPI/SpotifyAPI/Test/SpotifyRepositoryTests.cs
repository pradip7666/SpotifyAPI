using Moq;
using SpotifyAPI.Models;
using SpotifyAPI.Repository;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace SpotifyAPI.Tests
{
    public class SpotifyRepositoryTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly SpotifyRepository _repository;

        public SpotifyRepositoryTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _repository = new SpotifyRepository(_mockHttpClient.Object);
        }

        [Fact]
        public async Task IsAliveAsync_ReturnsTrue_WhenApiIsAlive()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            _mockHttpClient.Setup(client => client.GetAsync(It.IsAny<string>()))
                           .ReturnsAsync(responseMessage);

            // Act
            var result = await _repository.IsAliveAsync();

            // Assert
            Assert.True(result);
        }

        // Add more unit tests as needed
    }
}
