using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;
using Services.GiftServices.Managers;
using Services.GiftServices.Models;

namespace _77737CertiP2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly GiftManager _manager;
        private readonly string _url;
        public GiftController(GiftManager manager)
        {
            _manager = manager;
            _url = _manager.GetUrl();
        }

        [HttpGet("gifts/{id}")]
        public async Task<IActionResult> GetGiftById(string id)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{_url}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound($"Gift with ID {id} not found");
                }

                var gift = await response.Content.ReadFromJsonAsync<Gift>();
                return Ok(gift);
            }
            catch (Exception ex)
            {
                // Log the error (implementation depends on your logging setup)
                Console.WriteLine($"Error retrieving gift: {ex.Message}");
                return StatusCode(500, "Error retrieving gift");
            }
        }
    }
}
