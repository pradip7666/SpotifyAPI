using Microsoft.AspNetCore.Mvc;
using Moq;
using SpotifyAPI.Controllers;
using SpotifyAPI.Models;
using SpotifyAPI.Repository.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace SpotifyAPI.Tests
{
    public class SpotifyControllerTests
    {
        private readonly Mock<ISpotifyRepository> _mockRepo;
        private readonly SpotifyController _controller;

        public SpotifyControllerTests()
        {
            _mockRepo = new Mock<ISpotifyRepository>();
            _controller = new SpotifyController(_mockRepo.Object);
        }

        [Fact]
        public async Task SearchTopArtists_ReturnsOkResult_WithValidModel()
        {
            // Arrange
            var request = new SpotifyRequest { Query = "TestArtist" };
            _mockRepo.Setup(repo => repo.SearchTopArtistsAsync(request.Query))
                     .ReturnsAsync(new SpotifyResponse { Success = true, Artists = new List<Artist>() });

            // Act
            var result = await _controller.SearchTopArtists(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<SpotifyResponse>(okResult.Value);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task IsAlive_ReturnsOkResult()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.IsAliveAsync()).ReturnsAsync(true);

            // Act
            var result = await _controller.IsAlive();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResponseDTO>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Service is alive", response.Message);
        }
    }
}
