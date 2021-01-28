using DAF.AirplaneTrafficData.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DAF.AirplaneTrafficData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftController : ControllerBase
    {
        private readonly IAircraftService _aircraftService;

        public AircraftController(IAircraftService aircraftService)
        {
            _aircraftService = aircraftService;
        }
    }
}