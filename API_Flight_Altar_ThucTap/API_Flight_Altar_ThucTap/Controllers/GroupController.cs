using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroup _group;
        public GroupController(IGroup group)
        {
            _group = group;
        }

        [HttpGet("GetGroup")]
        public async Task<IActionResult> GetGroups()//Lấy danh sách group
        {
            try
            {
                return Ok(await _group.GetGroups());

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

        [HttpGet("GetMyGroup")]
        public async Task<IActionResult> GetMyGroupAsync()
        {
            try
            {
                var mygroup = await _group.GetMyGroup();
                return Ok(mygroup);
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

        [HttpPost("AddGroup")]
        public async Task<IActionResult> AddGroup(string groupName, string note)//Thêm group
        {
            try
            {
                var newGroup = await _group.CreateGroup(groupName, note);
                //return CreatedAtAction(nameof(AddGroup), new { id = newGroup.IdGroup }, newGroup);
                return Ok(newGroup);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, string groupName, string note)//cập nhật group
        {
            try
            {
                var updatedTypeDoc = await _group.UpdateGroup(id, groupName, note);
                return Ok(updatedTypeDoc);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(int id)//Xóa group
        {
            try
            {
                var group = await _group.DeleteGroup(id);
                return Ok(group);
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

        [HttpGet("FindGroup")]
        public async Task<IActionResult> FindGroup(string name)//Tìm group qua tên
        {
            try
            {
                var group = await _group.FindGroupByName(name);
                return Ok(group);
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
    }
}
