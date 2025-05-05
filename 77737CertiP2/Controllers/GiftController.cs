using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;
using Services.GiftServices.Managers;
using Services.GiftServices.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Serilog;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(string id)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{_url}/{id}");

                if (!response.IsSuccessStatusCode)
                    return NotFound($"Gift with ID {id} not found");

                // Manual deserialization for flexible handling
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var gift = JsonSerializer.Deserialize<Gift>(json, options);
                Log.Information("Gift retrieved successfully: {@Gift}", gift);
                return Ok(gift);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving gift");
                return StatusCode(500, "Error processing gift data");
            }
        }
    }
}
