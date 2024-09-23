namespace SpotifyAPI.Models
{
    public class Artist
    {
        public string Name { get; set; }
        public int Followers { get; set; }
       public string ImageUrl { get; set; }
       public string Genre { get; internal set; }
    }
}

