using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;
using Services.GiftServices.Managers;
using Services.GiftServices.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Serilog;
using ClinicLogic.Managers;
using Microsoft.AspNetCore.Identity;

namespace _77737CertiP2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly GiftManager _manager;
        private readonly PatientManager _patientManager;
        private readonly string _url;
        public GiftController(GiftManager managerG, PatientManager managerP)
        {
            _manager = managerG;
            _patientManager = managerP;
            _url = _manager.GetUrl();
        }


        // Async method to return a Gift by ID
        [HttpGet("{id}")]
        public async Task<Gift> GetGiftById(string id)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync($"{_url}/{id}");

                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Gift with ID {id} not found");

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var gift = JsonSerializer.Deserialize<Gift>(json, options);
                Log.Information("Gift retrieved successfully: {@Gift}", gift);
                return gift;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving gift");
                throw;
            }
        }


        [HttpPost("{patientCI}/{giftId}")]
        public IActionResult GiveGiftToPatient(string patientCI, string giftId)
        {
            try
            {
                // Verify patient exists
                _patientManager.GetPatient(patientCI);
                // Verify gift exists
                Gift gift = GetGiftById(giftId).Result;

                _manager.AddGiftToUser(patientCI,gift);
                Log.Information("Gift {GiftId} given to patient {PatientCI}", giftId, patientCI);
                return Ok($"Gift {giftId} assigned to patient {patientCI}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error giving gift to patient");
                return BadRequest(ex.Message);
            }
        }
        //see gifts from a specific user
        [HttpGet("patient/{id}")]
        public IActionResult GetUserGifts(string id)
        {
            try
            {
                var user = _manager.GetUser(id);
                if (user == null)
                {
                    Log.Warning("User with ID {Id} not found", id);
                    return NotFound($"User with ID {id} not found");
                }
                Log.Information("Gifts retrieved successfully for user {Id}", id);
                return Ok(user.gifts);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving gifts for user");
                return BadRequest(ex.Message);
            }
        }


        //see all users 
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _manager.GetUsers();
                Log.Information("All users retrieved successfully");
                return Ok(users);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving all users");
                return BadRequest(ex.Message);
            }
        }
    }
}
