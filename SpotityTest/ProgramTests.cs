using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyAPI.Tests
{
    [TestClass]
    public class ProgramTests
    {
        private static WebApplicationFactory<SpotifyAPI.Program> _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _factory = new WebApplicationFactory<SpotifyAPI.Program>();
            _client = _factory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task Test_Swagger_Endpoint()
        {
            // Act
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            // Assert
            response.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not a success code
            Assert.AreEqual("application/json", response.Content.Headers.ContentType.MediaType);
        }

        [TestMethod]
        public async Task Test_MiniProfiler_Endpoint()
        {
            // Act
            var response = await _client.GetAsync("/profiler/results-index");

            // Assert
            response.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not a success code
        }

        [TestMethod]
        public async Task Test_HealthCheck_Endpoint()
        {
            // Act
            var response = await _client.GetAsync("/health");

            // Assert
            response.EnsureSuccessStatusCode(); // This will throw an exception if the status code is not a success code
            Assert.AreEqual("text/plain", response.Content.Headers.ContentType.MediaType);
        }
    }
}

