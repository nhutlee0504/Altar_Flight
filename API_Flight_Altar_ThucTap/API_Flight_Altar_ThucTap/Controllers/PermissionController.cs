using API_Flight_Altar_ThucTap.Model;
using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly Services.IPermission _permission;

        public PermissionController(Services.IPermission permission)
        {
            _permission = permission;
        }

        [HttpGet("GetPermission")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                var per = await _permission.GetPermission();
                return Ok(per);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermission(string namePermission)
        {
            try
            {
                var per = await _permission.AddPermission(namePermission);
                return Ok(per);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("DeletePermission")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try
            {
                var per = await _permission.DeletePermission(id);
                return Ok(per);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
