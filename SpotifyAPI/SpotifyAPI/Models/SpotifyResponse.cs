using System.Collections.Generic;

namespace SpotifyAPI.Models
{
    public class SpotifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } // Add this property
        public List<Artist> Artists { get; set; }
    }
}
    
