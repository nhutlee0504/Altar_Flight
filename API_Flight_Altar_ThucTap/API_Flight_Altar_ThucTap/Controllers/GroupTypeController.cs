using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupTypeController : ControllerBase
    {
        private readonly IGroupType groupType;
        public GroupTypeController(IGroupType groupType)
        {
            this.groupType = groupType;
        }

        [HttpGet("GetGroupType")]
        public async Task<IActionResult> GetGroupType(int idType)
        {
            try
            {
                var gt = await groupType.GetGroupTypeByIdType(idType);
                return Ok(gt);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCastException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddGroupType")]
        public async Task<IActionResult> AddGroupType(int idGroup, int idType, int idPermission)
        {
            try
            {
                var gt = await groupType.AddGroupType(idGroup, idType, idPermission);
                return Ok(gt);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidCastException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
