using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Serilog;
using SpotifyAPI.Models;
using SpotifyAPI.Repository.Interfaces;
using SpotifyAPI.Web;
using StackExchange.Profiling;

// Repository/SpotifyRepository.cs

namespace SpotifyAPI.Repository
{
    public class SpotifyRepository : ISpotifyRepository
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;

        public SpotifyRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SpotifyResponse> SearchTopArtistsAsync(string query)
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                await GetAccessTokenAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{Constants.SpotifyApiBaseUrl}/search?q={query}&type=artist")
            {
                Headers = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken) }
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Log.Error("Error response from Spotify API: {ErrorContent}", errorContent);
                throw new HttpRequestException($"Error response from Spotify API: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var artists = json["artists"]["items"]
                .OrderByDescending(a => (int)a["followers"]["total"])
                .Take(5)
                .Select(a => new Artist
                {
                    Name = (string)a["name"],
                    Followers = (int)a["followers"]["total"]
                }).ToList();

            return new SpotifyResponse
            {
                Artists = artists,
                Success = true
            };
        }

        public async Task<bool> IsAliveAsync()
        {
            try
            {
                // Simple GET request to check if Spotify API is accessible
                var response = await _httpClient.GetAsync(Constants.SpotifyApiBaseUrl);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Content = new StringContent("grant_type=client_credentials", System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

            var clientId = "599eb145dd9241c4a9cb16f9477c1f";
            var clientSecret = "289b9d89c94d49102f349bff1fab88";
            var authHeaderValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeaderValue);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            _accessToken = json["access_token"].ToString();
        }
    }
}
