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
            var per = await _permission.GetPermission();
            return Ok(per);
        }

        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermission(string namePermission)
        {
            var per = await _permission.AddPermission(namePermission);
            return Ok(per);
        }

        [HttpDelete("DeletePermission")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var per = await _permission.DeletePermission(id);
            return Ok(per);
        }
    }
}
