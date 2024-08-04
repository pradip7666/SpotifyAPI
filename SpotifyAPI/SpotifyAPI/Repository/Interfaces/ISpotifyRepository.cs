using System.Threading.Tasks;
using SpotifyAPI.Models;

namespace SpotifyAPI.Repository.Interfaces
{
    public interface ISpotifyRepository
    {
        Task<SpotifyResponse> SearchTopArtistsAsync(string query);
        Task<bool> IsAliveAsync();
    }
}
