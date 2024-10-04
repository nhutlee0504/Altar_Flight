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

        [HttpGet]
        public async Task<IEnumerable<GroupModel>> GetGroups()
        {
            return await _group.GetGroups();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddGroup([FromBody] GroupDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newGroup = await _group.CreateGroup(model);
            return CreatedAtAction(nameof(AddGroup), new { id = newGroup.IdGroup }, newGroup);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] GroupDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTypeDoc = await _group.UpdateGroup(id, model);
                return Ok(updatedTypeDoc);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
