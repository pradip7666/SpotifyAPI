using Microsoft.AspNetCore.Mvc;
using Moq;
using SpotifyAPI.Controllers;
using SpotifyAPI.Models;
using SpotifyAPI.Repository;

namespace SpotifyAPI.Tests.Controllers
{
    [TestClass]
    public class SpotifyControllerTests
    {
        private readonly Mock<ISpotifyRepository> _spotifyRepositoryMock;
        private readonly SpotifyController _spotifyController;

        public SpotifyControllerTests()
        {
            _spotifyRepositoryMock = new Mock<ISpotifyRepository>();
            _spotifyController = new SpotifyController(_spotifyRepositoryMock.Object);
        }

        [TestMethod]
        public async Task SearchTopArtists_ReturnsOkResult_WhenArtistsFound()
        {
            // Arrange
            var request = new SpotifyRequest { Genre = "test" };
            var spotifyResponse = new SpotifyResponse
            {
                Artists = new List<Artist>
                {
                    new Artist { Name = "Artist 1", Followers = 1000 },
                    new Artist { Name = "Artist 2", Followers = 2000 }
                }
            };
            _spotifyRepositoryMock.Setup(repo => repo.SearchTopArtistsAsync(request.Genre))
                                  .ReturnsAsync(spotifyResponse);

            // Act
            var result = await _spotifyController.SearchTopArtists(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            var returnValue = okResult.Value as SpotifyResponse;
            Assert.IsNotNull(returnValue, "Expected SpotifyResponse.");
            Assert.AreEqual(2, returnValue.Artists.Count);
        }

        [TestMethod]
        public async Task SearchTopArtists_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new SpotifyRequest { Genre = "" };
            _spotifyController.ModelState.AddModelError("Query", "Required");

            // Act
            var result = await _spotifyController.SearchTopArtists(request);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult.");
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
        }

        [TestMethod]
        public async Task IsAlive_ReturnsOkResult_WhenServiceIsAlive()
        {
            // Arrange
            _spotifyRepositoryMock.Setup(repo => repo.IsAliveAsync())
                                  .ReturnsAsync(true);

            // Act
            var result = await _spotifyController.IsAlive();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            var returnValue = okResult.Value as ResponseDTO;
            Assert.IsNotNull(returnValue, "Expected ResponseDTO.");
            Assert.IsTrue(returnValue.Success);
            Assert.AreEqual("Service is alive", returnValue.Message);
        }

        [TestMethod]
        public async Task IsAlive_ReturnsOkResult_WhenServiceIsDown()
        {
            // Arrange
            _spotifyRepositoryMock.Setup(repo => repo.IsAliveAsync())
                                  .ReturnsAsync(false);

            // Act
            var result = await _spotifyController.IsAlive();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult.");
            var returnValue = okResult.Value as ResponseDTO;
            Assert.IsNotNull(returnValue, "Expected ResponseDTO.");
            Assert.IsFalse(returnValue.Success);
            Assert.AreEqual("Service is down", returnValue.Message);
        }
    }
}
