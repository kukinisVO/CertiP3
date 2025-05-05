using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;
using Services.GiftServices.Managers;

namespace _77737CertiP2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly GiftManager _manager;
        public GiftController(GiftManager manager)
        {
            _manager = manager;
        }
    }
}
