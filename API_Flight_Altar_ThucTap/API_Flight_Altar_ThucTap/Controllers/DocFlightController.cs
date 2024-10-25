using API_Flight_Altar_ThucTap.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_Flight_Altar_ThucTap.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocFlightController : ControllerBase
    {
        private readonly IDocFlight _docFlight;
        public DocFlightController(IDocFlight docFlight)
        {
            _docFlight = docFlight;
        }

        [HttpGet("GetAllDocFlight")]
        public async Task<IActionResult> GetAllDocFlight()
        {
            try
            {
                var getF = await _docFlight.GetAllDocFlight();
                return Ok(getF);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] int flightId, [FromForm] int typeId)
        {
            // Kiểm tra file có hợp lệ không
            if (file == null || file.Length == 0)
            {
                return BadRequest("Chọn file để tải lên.");
            }

            try
            {
                // Gọi phương thức AddDocFlight từ service
                var docFlight = await _docFlight.AddDocFlight(file, flightId, typeId);
                return Ok(docFlight); // Trả về thông tin tài liệu đã thêm
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Trả về thông báo lỗi nếu có
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download/{idDocument}")]
        public async Task<IActionResult> DownloadFile(int idDocument)
        {
            try
            {
                var docFlightDto = await _docFlight.DownloadDocFlight(idDocument);
                var fileName = docFlightDto.DocumentName; // Đặt tên file từ DocumentName

                return File(docFlightDto.FileContent, "application/octet-stream", fileName); // Trả về file
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message); // Trả về 404 nếu không tìm thấy file
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("update/{idDocFlight}")]
        public async Task<IActionResult> UpdateFile(int idDocFlight, IFormFile file)
        {
            // Check if the file is valid
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file to upload.");
            }

            try
            {
                // Call UpdateDocFlight from the service
                var updatedDocFlight = await _docFlight.UpdateDocFlight(idDocFlight, file);
                return Ok(updatedDocFlight); // Return updated document info
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Return error message if any
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message); // Return 404 if file not found
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDocFlight([FromQuery] string searchTerm)
        {
            try
            {
                var documents = await _docFlight.SearchDocFlight(searchTerm);
                return Ok(documents);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove/{idDocument}")]
        public async Task<IActionResult> RemoveDocument(int idDocument)
        {
            try
            {
                var removedDocFlight = await _docFlight.RemoveFlight(idDocument);
                return Ok(removedDocFlight);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("confirm/{idDocument}")]
        public async Task<IActionResult> ConfirmDocument(int idDocument)
        {
            try
            {
                var confirmedDoc = await _docFlight.ConfirmDocFlight(idDocument);
                return Ok(confirmedDoc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
