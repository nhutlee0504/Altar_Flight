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
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
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
            throw new UnauthorizedAccessException("You do not have permission");
        }
        public async Task<DocFlightDto> DownloadDocFlight(int idDocument)
        {
            var docFlight = await _context.documents.FindAsync(idDocument);
            if (docFlight == null || docFlight.IsDeleted == true)
            {
                throw new FileNotFoundException("Document does not exist");
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
            throw new UnauthorizedAccessException("You do not have permission");
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
        public async Task<IEnumerable<DocFlight>> SearchDocFlight(string searchTerm)
        {
            var userInfo = GetUserInfoFromClaims();

            if (!userInfo.Role.ToLower().Contains("admin") && !userInfo.Role.ToLower().Contains("go"))
            {
                throw new UnauthorizedAccessException("You do not have permission to search documents.");
            }

            var documents = await _context.documents
                .Where(doc => doc.DocumentName.Contains(searchTerm) || doc.IdDocument.ToString() == searchTerm)
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

        public async Task<DocFlight> UpdateDocFlight(int idDocFlight, IFormFile formFile)
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower() == "admin" || userInfo.Role.ToLower() == "go")
            {
                var existingDoc = await _context.documents.FindAsync(idDocFlight);
                if (existingDoc == null || existingDoc.IsDeleted == true)
                {
                    throw new FileNotFoundException("Document not found.");
                }

                if (existingDoc.Signature != null)
                {
                    throw new InvalidOperationException("Cannot update document because its status is Confirmed");
                }

                if (formFile == null || formFile.Length == 0)
                {
                    throw new ArgumentException("Invalid file.");
                }

                var allowedExtensions1 = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension1 = Path.GetExtension(formFile.FileName).ToLower();

                if (!allowedExtensions1.Contains(fileExtension1))
                {
                    throw new ArgumentException("File must be in DOC, DOCX, or PDF format.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    var fileContent = memoryStream.ToArray(); 

                    var newDoc = new DocFlight
                    {
                        DocumentName = existingDoc.DocumentName,
                        FileContent = fileContent,
                        Version = Math.Round((double)(existingDoc.Version + 0.1), 1),
                        CreatedTime = existingDoc.CreatedTime,
                        UpdatedTime = DateTime.UtcNow,
                        Status = "Updated",
                        IsDeleted = false,
                        UserId = userInfo.IdUser,
                        FlightId = existingDoc.FlightId,
                        TypeId = existingDoc.TypeId,
                    };

                    await _context.documents.AddAsync(newDoc);
                    await _context.SaveChangesAsync();
                    return newDoc; 
                }
            }

            var existingDocument = await _context.documents.FindAsync(idDocFlight);
            if (existingDocument == null || existingDocument.IsDeleted == true)
            {
                throw new FileNotFoundException("Document not found.");
            }

            if (existingDocument.Status.ToLower().Contains("confirmed"))
            {
                throw new InvalidOperationException("Cannot update document because its status is 'Đã xác nhận'.");
            }

            var userGroups = await _context.group_Users
                .Where(gu => gu.UserID == userInfo.IdUser)
                .Select(gu => gu.GroupID)
                .ToListAsync();

            var accessibleTypeDocs = await _context.group_Types
                .Where(gt => userGroups.Contains(gt.IdGroup)
                    && gt.Permission.PermissionName == "Read and modify")
                .Select(gt => gt.IdType)
                .ToListAsync();

            if (!accessibleTypeDocs.Contains(existingDocument.TypeId))
            {
                throw new UnauthorizedAccessException("You do not have permission to update this document.");
            }

            if (formFile == null || formFile.Length == 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            var allowedExtensions2 = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension2 = Path.GetExtension(formFile.FileName).ToLower();

            if (!allowedExtensions2.Contains(fileExtension2))
            {
                throw new ArgumentException("File must be in DOC, DOCX, or PDF format.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                var newDoc = new DocFlight
                {
                    DocumentName = existingDocument.DocumentName,
                    FileContent = fileContent,
                    Version = Math.Round((double)(existingDocument.Version + 0.1), 1),
                    CreatedTime = existingDocument.CreatedTime,
                    UpdatedTime = DateTime.UtcNow,
                    Status = "Updated",
                    IsDeleted = false,
                    UserId = userInfo.IdUser,
                    FlightId = existingDocument.FlightId,
                    TypeId = existingDocument.TypeId,
                };

                await _context.documents.AddAsync(newDoc);
                await _context.SaveChangesAsync();
                return newDoc;
            }
        }

        public async Task<DocFlight> RemoveFlight(int idDocument)
        {
            var userInfo = GetUserInfoFromClaims();

            var docFlight = await _context.documents.FirstOrDefaultAsync(x => x.IdDocument == idDocument);
            if (docFlight == null || docFlight.IsDeleted == true)
            {
                throw new FileNotFoundException("Document not found or already deleted.");
            }
            if (docFlight.Signature != null)
            {
                throw new InvalidOperationException("Cannot update document because its status is Confirmed");
            }
            if (userInfo.Role.ToLower().Contains("admin"))
            {
                docFlight.IsDeleted = true;
                await _context.SaveChangesAsync();
                return docFlight;
            }
            if (docFlight.UserId != userInfo.IdUser)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this document.");
            }

            // Đánh dấu tài liệu là đã xóa
            docFlight.IsDeleted = true;
            await _context.SaveChangesAsync();

            return docFlight; // Trả về tài liệu đã xóa
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

        public async Task<DocFlight> ConfirmDocFlight(int idDocument)
        {
            var userInfo = GetUserInfoFromClaims();

            // Kiểm tra quyền truy cập
            if (!userInfo.Role.ToLower().Contains("admin"))
            {
                throw new UnauthorizedAccessException("You do not have permission to confirm this document.");
            }

            var docFlight = await _context.documents.FindAsync(idDocument);
            if (docFlight == null || docFlight.IsDeleted == true)
            {
                throw new FileNotFoundException("Document not found.");
            }

            // Cập nhật trạng thái và chữ ký
            docFlight.Status = "Confirmed";
            docFlight.Signature = userInfo.Email; // Hoặc userInfo.Name nếu có thuộc tính này
            docFlight.UpdatedTime = DateTime.UtcNow;

            _context.documents.Update(docFlight);
            await _context.SaveChangesAsync();

            return docFlight; // Trả về tài liệu đã được xác nhận
        }

    }
}
