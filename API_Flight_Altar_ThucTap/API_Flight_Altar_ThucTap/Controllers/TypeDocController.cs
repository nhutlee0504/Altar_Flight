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
    public class TypeDocController : ControllerBase
    {
        private readonly ITypeDoc _typeDocService;

        public TypeDocController(ITypeDoc typeDocService)
        {
            _typeDocService = typeDocService;
        }

        [HttpGet("TypeDoc")]
        public async Task<IActionResult> GetTypeDocs()//Lấy tất cả loại tài liệu
        {
            try
            {
                return Ok(await _typeDocService.GetTypeDocs());

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

        [HttpGet("GetMyType")]
        public async Task<IActionResult> GetMyType()
        {
            try
            {
                var myt = await _typeDocService.GetMyTypeDoc();
                return Ok(myt);
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

        [HttpPost("AddTypeDoc")]
        public async Task<IActionResult> AddTypeDoc(string typeName, string Note)//Thêm loại tài liệu
        {
            try
            {
                var typeDoc = await _typeDocService.AddTypeDoc(typeName, Note);
                //return CreatedAtAction(nameof(AddTypeDoc), new { id = typeDoc.IdTypeDoc }, typeDoc);
                return Ok(typeDoc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(NotImplementedException ex)
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

        [HttpGet("FindTypeByName")]
        public async Task<IActionResult> FindTypeByName(string name)//Tìm typeDoc theo Id
        {
            try
            {
                var typeDoc = await _typeDocService.FindTypeDocByName(name);
                return Ok(typeDoc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
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

        [HttpGet("FindTypeById")]
        public async Task<IActionResult> FindTypeById(int idTypeDoc)
        {
            try
            {
                var type = await _typeDocService.FindTypeDocById(idTypeDoc);
                return Ok(type);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypeDoc(int id, string typeName, string Note)//Cập nhật loại tài liệu
        {
            try
            {
                var updatedTypeDoc = await _typeDocService.UpdateTypeDoc(id, typeName, Note);
                return Ok(updatedTypeDoc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return NotFound(ex.Message);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeDoc(int id)//Xóa loại tài liệu
        {
            try
            {
                var typeDoc = await _typeDocService.DeleteTypeDoc(id);
                return Ok(typeDoc);
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
