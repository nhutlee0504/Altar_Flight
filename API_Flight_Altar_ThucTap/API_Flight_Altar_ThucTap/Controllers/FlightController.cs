using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private IFlight _flight;
        public FlightController(IFlight flight)
        {
            _flight = flight;
        }

        [HttpGet("GetAllFlight")]
        public async Task<IActionResult> GetAllFlightAsync()
        {
            try
            {
                var getFlight = await _flight.GetAllFlight();
                return Ok(getFlight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetMyFlight")]
        public async Task<IActionResult> GetMyFlight()
        {
            try
            {
                var getMyFlight = await _flight.GetMyFlight();
                return Ok(getMyFlight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddFlight")]
        public async Task<IActionResult> AddFlight(FlightInfo flightInfo)
        {
            try
            {
                var addFlight = await _flight.AddFlight(flightInfo);
                return Ok(addFlight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateFlight")]
        public async Task<IActionResult> UpdateFlight(int idFlight, FlightInfo flightInfo)
        {
            try
            {
                var UpFlight = await _flight.UpdateFlight(idFlight, flightInfo);
                return Ok(UpFlight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteFlight")]
        public async Task<IActionResult> DeleteFlight(int idFlight)
        {
            try
            {
                var delFlight = await _flight.RemoveFlight(idFlight);
                return Ok(delFlight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotFiniteNumberException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetFlightByname")]
        public async Task<IActionResult> GetFlightByName(string name)
        {
            try
            {
                var flight = await _flight.GetFlightByName(name);
                return Ok(flight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetFlightById")]
        public async Task<IActionResult> GetFlightById(int id)
        {
            try
            {
                var flight = await _flight.GetFlightById(id);
                return Ok(flight);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
