using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Models;
using SpotifyAPI.Repository.Interfaces;

namespace SpotifyAPI.Repositories
{
    public class SpotifyRepository : ISpotifyRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _accessToken;
        private DateTime _tokenExpiration;

        public SpotifyRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<SpotifyResponse> SearchTopArtistsAsync(string query)
        {
            await EnsureAccessTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, Constants.SpotifySearchEndpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { q = query, type = "artist", limit = 5 }), System.Text.Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(content);

            var artists = jsonResponse["artists"]["items"]
                .Select(item => new Artist
                {
                    Name = item["name"].ToString(),
                    Followers = int.Parse(item["followers"]["total"].ToString()),
                   // Genre = item["genres"].Any() ? item["genres"].First().ToString() : "Unknown"
                })
                .OrderByDescending(a => a.Followers)
                .Take(5)
                .ToList();

            return new SpotifyResponse { Artists = artists };
        }

        private async Task EnsureAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiration)
            {
                var clientId = _configuration["Spotify:ClientId"];
                var clientSecret = _configuration["Spotify:ClientSecret"];

                var tokenRequest = new HttpRequestMessage(HttpMethod.Post, Constants.SpotifyTokenUrl);
                tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));
                tokenRequest.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });

                var tokenResponse = await _httpClient.SendAsync(tokenRequest);
                tokenResponse.EnsureSuccessStatusCode();

                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var tokenResult = JsonConvert.DeserializeObject<TokenResponse>(tokenContent);

                _accessToken = tokenResult.AccessToken;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(tokenResult.ExpiresIn);
            }
        }

        Task<bool> ISpotifyRepository.IsAliveAsync()
        {
            throw new NotImplementedException();
        }

        Task<SpotifyResponse> ISpotifyRepository.SearchTopArtistsAsync(string query)
        {
            throw new NotImplementedException();
        }
    }
}
