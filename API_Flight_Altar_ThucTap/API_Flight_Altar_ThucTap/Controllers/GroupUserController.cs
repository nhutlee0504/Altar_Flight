using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupUserController : ControllerBase
    {
        private readonly IGroupUser _groupUser;
        public GroupUserController(IGroupUser groupUser)
        {
            _groupUser = groupUser;
        }

        [HttpPost]
        public async Task<IActionResult> AddGroupUser(int GroupID, int UserID)
        {
            try
            {
                var gu = await _groupUser.AddGroupUser(GroupID, UserID);
                return Ok(gu);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotImplementedException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupUser(int id)
        {
            var gu = await _groupUser.GetGroupUserById(id);
            return Ok(gu);
        }

        [HttpDelete("DeleteGroupUser")]
        public async Task<IActionResult> DeleteGroupUser(int groupID, int userID)
        {
            try
            {
                var gu = await _groupUser.DeleteGroupUser(groupID, userID);
                return Ok(gu);
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
