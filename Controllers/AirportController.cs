using DAF.AirplaneTrafficData.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DAF.AirplaneTrafficData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService _airportService;

        public AirportController(IAirportService airportService)
        {
            _airportService = airportService;
        }
    }
}