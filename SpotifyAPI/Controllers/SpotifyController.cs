using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Filters;
using SpotifyAPI.Models;
using SpotifyAPI.Repository;
using StackExchange.Profiling;
using System.Threading.Tasks;

namespace SpotifyAPI.Controllers
{
    [Route("api/spotify")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly ISpotifyRepository _spotifyRepository;

        public SpotifyController(ISpotifyRepository spotifyRepository)
        {
            _spotifyRepository = spotifyRepository;
        }

        [HttpPost("search")]
        [ValidateModel]
        public async Task<IActionResult> SearchTopArtists([FromBody] SpotifyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Ensure to return BadRequest if ModelState is invalid
            }
         
            using (MiniProfiler.Current.Step("SpotifyController.SearchTopArtists"))
            {
                var result = await _spotifyRepository.SearchTopArtistsAsync(request.Genre);
                return Ok(result);
            }
        }

        [HttpGet("isalive")]
        public async Task<IActionResult> IsAlive()
        {
            using (MiniProfiler.Current.Step("SpotifyController.IsAlive"))
            {
                var result = await _spotifyRepository.IsAliveAsync();
                return Ok(new ResponseDTO { Success = result, Message = result ? "Service is alive" : "Service is down" });
            }
        }
    }
}
