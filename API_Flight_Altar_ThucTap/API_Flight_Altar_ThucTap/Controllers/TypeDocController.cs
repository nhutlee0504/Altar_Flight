using API_Flight_Altar_ThucTap.Model;
using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeDocController : ControllerBase
    {
        private readonly ITypeDoc _typeDocService;

        public TypeDocController(ITypeDoc typeDocService)
        {
            _typeDocService = typeDocService;
        }

        [HttpGet]
        public async Task<IEnumerable<TypeDoc>> GetTypeDocs()
        {
            return await _typeDocService.GetTypeDocs();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTypeDoc([FromBody] TypeDocDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var typeDoc = await _typeDocService.AddTypeDoc(model);
            return CreatedAtAction(nameof(AddTypeDoc), new { id = typeDoc.IdTypeDoc }, typeDoc);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTypeDoc(int id, [FromBody] TypeDocDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedTypeDoc = await _typeDocService.UpdateTypeDoc(id, model);
                return Ok(updatedTypeDoc);
            }
            catch (NotImplementedException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
