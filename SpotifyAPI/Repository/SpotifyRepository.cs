using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Models;
using SpotifyAPI.Web.Http;

namespace SpotifyAPI.Repository
{
    public class SpotifyRepository : ISpotifyRepository
    {
        public const string ClientId = "599eb145dd9241c4a9cb1776f9477c1f";
        public const string ClientSecret = "380bfd5f509c4c88829168459fa1b02e";

        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private string _token;
        private DateTime _tokenExpiry;

        public SpotifyRepository(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            _client = client;
        }
        
        public async Task<string> GetTokenAsync()
        {
            if (_token != null && DateTime.Now < _tokenExpiry)
            {
                return _token;
            }

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

            // Create HTTP request
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                request.Content = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

                // Send request and get response
                using (HttpResponseMessage response = await _client.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException($"Token request failed with status code {response.StatusCode} and message: {errorContent}");
                    }

                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Parse JSON response
                    var json = JObject.Parse(responseBody);
                    _token = json["access_token"].ToString();
                    _tokenExpiry = DateTime.Now.AddSeconds(json["expires_in"].ToObject<int>());

                    return _token;
                }
            }
        }

        public async Task<bool> IsAliveAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, Constants.SpotifyApiBaseUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await _client.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public async Task<SpotifyResponse> SearchTopArtistsAsync(string Genre)
        {
            var token = await GetTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestUri = $"https://api.spotify.com/v1/search?q=genre:{Genre}&type=artist&limit=50";
            try
            {
                var response = await _client.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Search request failed with status code {response.StatusCode} and message: {errorContent}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                var artists = json["artists"]["items"]
                    .Select(a => new Artist
                    {
                        Name = a["name"].ToString(),
                        Followers = a["followers"]["total"].ToObject<int>(),
                        ImageUrl = a["images"].Select(i => i["url"].ToString()).FirstOrDefault(),
                        Genre = a["genres"].Any() ? a["genres"].First().ToString() : "Unknown"
                    })
                    .OrderByDescending(a => a.Followers)
                    .Take(5) // Adjusted to match limit
                    .ToList();

                return new SpotifyResponse { Artists = artists };
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Failed to search for artists", ex);
            }
        }

    }
}

