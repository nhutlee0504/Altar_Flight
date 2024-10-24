using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class DocFlightService : IDocFlight
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocFlightService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<DocFlight> AddDocFlight(IFormFile formFile, int Flightid, int Typeid)
        {
            // Lấy thông tin người dùng từ Claims
            var userInfo = GetUserInfoFromClaims();

            // Kiểm tra file có hợp lệ không
            if (formFile == null || formFile.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ.");
            }

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(formFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("File phải có định dạng DOC, DOCX hoặc PDF.");
            }

            // Đọc nội dung file vào mảng byte
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray(); // Lưu nội dung file

                var newDoc = new DocFlight
                {
                    DocumentName = formFile.FileName, // Lấy tên file
                    FileContent = fileContent, // Lưu nội dung file
                    Version = 1.0,
                    CreatedTime = DateTime.UtcNow,
                    UpdatedTime = DateTime.UtcNow,
                    Status = "Uploaded",
                    IsDeleted = false,
                    UserId = userInfo.IdUser,
                    FlightId = Flightid,// Cập nhật FlightId nếu cần
                    TypeId = Typeid,

                };

                await _context.documents.AddAsync(newDoc);
                await _context.SaveChangesAsync();
                return newDoc; // Trả về đối tượng đã thêm
            }
        }

        public async Task<DocFlightDto> DownloadDocFlight(int idDocument)
        {
            var docFlight = await _context.documents.FindAsync(idDocument);
            if (docFlight == null || docFlight.IsDeleted == true)
            {
                throw new FileNotFoundException("Tài liệu không tồn tại.");
            }

            // Chuyển đổi sang DTO
            var docFlightDto = new DocFlightDto
            {
                IdDocument = docFlight.IdDocument,
                DocumentName = docFlight.DocumentName,
                FileContent = docFlight.FileContent,
                CreatedTime = docFlight.CreatedTime,
                UpdatedTime = docFlight.UpdatedTime,
                Status = docFlight.Status
            };

            return docFlightDto; // Trả về đối tượng DTO
        }


        public async Task<IEnumerable<DocFlight>> GetAllDocFlight()
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role == "Admin")
            {
                var documents = await _context.documents
            .Select(doc => new DocFlight
            {
                IdDocument = doc.IdDocument,
                DocumentName = doc.DocumentName,
                Version = doc.Version,
                CreatedTime = doc.CreatedTime,
                UpdatedTime = doc.UpdatedTime,
                Status = doc.Status,
                Signature = doc.Signature,
                IsDeleted = doc.IsDeleted,
                UserId = doc.UserId,
                FlightId = doc.FlightId,
                TypeId = doc.TypeId,
            })
            .ToListAsync();

                return documents;
            }
            throw new UnauthorizedAccessException("Only Admin");
        }

        public async Task<IEnumerable<DocFlight>> GetDocFlightByIdFlight(int idFlight)
        {
            var userInfo = GetUserInfoFromClaims();
            var documents = await _context.documents
            .Where(doc => doc.FlightId == idFlight) // Thêm điều kiện where
            .Select(doc => new DocFlight
            {
                IdDocument = doc.IdDocument,
                DocumentName = doc.DocumentName,
                Version = doc.Version,
                CreatedTime = doc.CreatedTime,
                UpdatedTime = doc.UpdatedTime,
                Status = doc.Status,
                Signature = doc.Signature,
                IsDeleted = doc.IsDeleted,
                UserId = doc.UserId,
                FlightId = doc.FlightId,
                TypeId = doc.TypeId,
            })
            .ToListAsync();

            return documents;
        }

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;

            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var expClaim = userClaim.FindFirst("exp");
                if (expClaim != null && long.TryParse(expClaim.Value, out long exp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                    if (expirationTime < DateTime.UtcNow)
                    {
                        throw new UnauthorizedAccessException("Token has expired. Please log in again");
                    }
                }

                var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

                if (idClaim != null && emailClaim != null && roleClaim != null)
                {
                    return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
                }
            }

            throw new UnauthorizedAccessException("Please log in to the system");
        }

    }
}
