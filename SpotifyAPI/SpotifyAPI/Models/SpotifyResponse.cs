using System.Collections.Generic;

namespace SpotifyAPI.Models
{
    public class SpotifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } // Add this property
        public List<Artist> Artists { get; set; }
        public string Id { get; internal set; }
        public string Name { get; internal set; }
        public int Followers { get; internal set; }
    }
}
    
