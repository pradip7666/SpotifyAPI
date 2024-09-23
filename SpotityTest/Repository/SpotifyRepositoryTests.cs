using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Models;
using SpotifyAPI.Repository;

namespace SpotifyAPI.Tests.Repository
{
    [TestClass]
    public class SpotifyRepositoryTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly SpotifyRepository _spotifyRepository;

        public SpotifyRepositoryTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _spotifyRepository = new SpotifyRepository(_configurationMock.Object, _httpClient);
        }

        [TestMethod]
        public async Task GetTokenAsync_ReturnsToken_WhenCalledForTheFirstTime()
        {
            // Arrange
            var tokenResponse = new JObject
            {
                { "access_token", "test_access_token" },
                { "expires_in", 3600 }
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse.ToString())
                });

            // Act
            var token = await _spotifyRepository.GetTokenAsync();

            // Assert
            Assert.AreEqual("test_access_token", token);
        }

        [TestMethod]
        public async Task GetTokenAsync_ReturnsExistingToken_WhenTokenIsValid()
        {
            // Arrange
            var tokenResponse = new JObject
            {
                { "access_token", "test_access_token" },
                { "expires_in", 3600 }
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse.ToString())
                });

            // Act - Call GetTokenAsync twice
            var firstToken = await _spotifyRepository.GetTokenAsync();
            var secondToken = await _spotifyRepository.GetTokenAsync();

            // Assert
            Assert.AreEqual("test_access_token", firstToken);
            Assert.AreEqual("test_access_token", secondToken);
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(), // Token request should be made only once
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [TestMethod]
        public async Task GetTokenAsync_ThrowsHttpRequestException_WhenTokenRequestFails()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _spotifyRepository.GetTokenAsync());
        }

        [TestMethod]
        public async Task IsAliveAsync_ReturnsTrue_WhenApiIsAlive()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            // Act
            var isAlive = await _spotifyRepository.IsAliveAsync();

            // Assert
            Assert.IsTrue(isAlive);
        }

        [TestMethod]
        public async Task IsAliveAsync_ReturnsFalse_WhenApiIsDown()
        {
            // Arrange
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act
            var isAlive = await _spotifyRepository.IsAliveAsync();

            // Assert
            Assert.IsFalse(isAlive);
        }

        [TestMethod]
        public async Task SearchTopArtistsAsync_ReturnsArtists_WhenQueryIsSuccessful()
        {
            // Arrange
            var tokenResponse = new JObject
            {
                { "access_token", "test_access_token" },
                { "expires_in", 3600 }
            };

            var searchResponse = new JObject
            {
                { "artists", new JObject
                    {
                        { "items", new JArray
                            {
                                new JObject
                                {
                                    { "name", "Artist 1" },
                                    { "followers", new JObject { { "total", 1000 } } },
                                    { "images", new JArray { new JObject { { "url", "image_url_1" } } } },
                                    { "genres", new JArray { "Genre1" } }
                                },
                                new JObject
                                {
                                    { "name", "Artist 2" },
                                    { "followers", new JObject { { "total", 2000 } } },
                                    { "images", new JArray { new JObject { { "url", "image_url_2" } } } },
                                    { "genres", new JArray { "Genre2" } }
                                }
                            }
                        }
                    }
                }
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse.ToString())
                });

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(searchResponse.ToString())
                });

            // Act
            var result = await _spotifyRepository.SearchTopArtistsAsync("test_query");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Artists.Count);
            Assert.AreEqual("Artist 2", result.Artists.First().Name);
            Assert.AreEqual(2000, result.Artists.First().Followers);
        }

        [TestMethod]
        public async Task SearchTopArtistsAsync_ThrowsException_WhenApiFails()
        {
            // Arrange
            var tokenResponse = new JObject
            {
                { "access_token", "test_access_token" },
                { "expires_in", 3600 }
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(tokenResponse.ToString())
                });

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Internal Server Error")
                });

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _spotifyRepository.SearchTopArtistsAsync("test_query"));
        }
    }
}
