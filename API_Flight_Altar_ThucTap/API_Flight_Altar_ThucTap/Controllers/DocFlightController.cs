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

    }
}
