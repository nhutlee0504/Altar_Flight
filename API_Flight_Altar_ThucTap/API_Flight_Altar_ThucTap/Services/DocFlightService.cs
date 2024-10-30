using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Claims;

namespace API_Flight_Altar_ThucTap.Services
{
    public class DocFlightService : IDocFlight
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();
        public DocFlightService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<DocFlight>> GetAllDocFlight()//Lấy tất cả tài liệu của tất cả các chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin"))
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
        public async Task<IEnumerable<DocFlight>> GetDocFlightByIdFlight(int idFlight)//Lấy danh sách tài liệu chuyến bay theo Id của Flight
        {
            var userInfo = GetUserInfoFromClaims();
            var documents = await _context.documents
          .Where(doc => doc.FlightId == idFlight)
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
            if (userInfo.Role.ToLower().Contains("admin"))
            {

                return documents;
            }
            throw new UnauthorizedAccessException();

        }
        public async Task<DocFlight> AddDocFlight(IFormFile formFile, int Flightid, int Typeid)//Thêm tài liệu cho chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                if (formFile == null || formFile.Length == 0)
                {
                    throw new ArgumentException("Invalid file.");
                }

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(formFile.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException("File must be in DOC, DOCX, or PDF format.");
                }

                var typeDoc = await _context.typeDocs.FindAsync(Typeid);
                if (typeDoc == null)
                {
                    throw new ArgumentException("TypeDoc does not exist.");
                }

                if (typeDoc.Status?.ToLower() == "deleted")
                {
                    throw new InvalidOperationException("Cannot add document because the TypeDoc status is 'Deleted'.");
                }

                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    var fileContent = memoryStream.ToArray();

                    var newDoc = new DocFlight
                    {
                        DocumentName = formFile.FileName,
                        FileContent = fileContent,
                        Version = 1.0,
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow,
                        Status = "Uploaded",
                        IsDeleted = false,
                        UserId = userInfo.IdUser,
                        FlightId = Flightid,
                        TypeId = Typeid,
                    };

                    await _context.documents.AddAsync(newDoc);
                    await _context.SaveChangesAsync();
                    return newDoc;
                }
            }

            throw new UnauthorizedAccessException("You do not have permission.");
        }
        public async Task<DocFlightDto> DownloadDocFlight(int idDocument) // Download flight document
        {
            var userInfo = GetUserInfoFromClaims();
            var docFlight = await _context.documents.FindAsync(idDocument);

            if (docFlight == null || docFlight.IsDeleted == true)
            {
                throw new FileNotFoundException("Document does not exist.");
            }
            if (userInfo.Role.ToLower().Contains("admin"))
            {
                return new DocFlightDto
                {
                    IdDocument = docFlight.IdDocument,
                    DocumentName = docFlight.DocumentName,
                    FileContent = docFlight.FileContent,
                    CreatedTime = docFlight.CreatedTime,
                    UpdatedTime = docFlight.UpdatedTime,
                    Status = docFlight.Status
                };
            }

            var flight = await _context.flights.FirstOrDefaultAsync(f => f.IdFlight == docFlight.FlightId);
            if (flight == null)
            {
                throw new FileNotFoundException("Associated flight not found.");
            }

            if (flight.UserId == userInfo.IdUser)
            {
                return new DocFlightDto
                {
                    IdDocument = docFlight.IdDocument,
                    DocumentName = docFlight.DocumentName,
                    FileContent = docFlight.FileContent,
                    CreatedTime = docFlight.CreatedTime,
                    UpdatedTime = docFlight.UpdatedTime,
                    Status = docFlight.Status
                };
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

            if (!accessibleTypeDocs.Contains(docFlight.TypeId))
            {
                throw new UnauthorizedAccessException("You do not have permission to download this document.");
            }

            return new DocFlightDto
            {
                IdDocument = docFlight.IdDocument,
                DocumentName = docFlight.DocumentName,
                FileContent = docFlight.FileContent,
                CreatedTime = docFlight.CreatedTime,
                UpdatedTime = docFlight.UpdatedTime,
                Status = docFlight.Status
            };
        }
        public async Task<IEnumerable<DocFlight>> SearchDocFlight(string searchTerm)//Tìm kiếm tài liệu chuyến bay
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
        public async Task<DocFlight> UpdateDocFlight(int idDocFlight, IFormFile formFile)//Cập nhật tài liệu chuyến bya
        {
            var userInfo = GetUserInfoFromClaims();

            if (userInfo.Role.ToLower() == "admin" || userInfo.Role.ToLower() == "go")
            {
                var existingDoc = await _context.documents.FindAsync(idDocFlight);
                if (existingDoc == null || existingDoc.IsDeleted == true)
                {
                    throw new FileNotFoundException("Document not found");
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
                    throw new ArgumentException("File must be in DOC, DOCX, or PDF format");
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
                throw new InvalidOperationException("Cannot update document because its status is Confirmed.");
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
        public async Task<DocFlight> RemoveFlight(int idDocument) // Xóa tài liệu chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();

            var docFlight = await _context.documents.FirstOrDefaultAsync(x => x.IdDocument == idDocument);
            if (docFlight == null || docFlight.IsDeleted == true)
            {
                throw new FileNotFoundException("Document not found or already deleted.");
            }

            if (docFlight.Signature != null)
            {
                throw new InvalidOperationException("Cannot delete document because its status is Confirmed");
            }

            if (userInfo.Role.ToLower().Contains("admin"))
            {
                docFlight.IsDeleted = true;
                await _context.SaveChangesAsync();
                return docFlight;
            }
            var flight = await _context.flights.FirstOrDefaultAsync(f => f.IdFlight == docFlight.FlightId);
            if (flight != null && flight.UserId == userInfo.IdUser)
            {
                docFlight.IsDeleted = true;
                await _context.SaveChangesAsync();
                return docFlight;
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

            if (!accessibleTypeDocs.Contains(docFlight.TypeId))
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this document.");
            }

            docFlight.IsDeleted = true;
            await _context.SaveChangesAsync();

            return docFlight;
        }

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;

            if (userClaim == null || !userClaim.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("Please log in to the system");
            }

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (_blacklistedTokens.Contains(token))
            {
                throw new UnauthorizedAccessException("Token has been invalidated.");
            }

            var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
            var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
            var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

            if (idClaim == null || emailClaim == null || roleClaim == null)
            {
                throw new InvalidOperationException("User claims are missing.");
            }

            return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
        }

        public async Task<DocFlight> ConfirmDocFlight(int idDocument)//Xác nhận tài liệu của chuyến bay
        {
            var userInfo = GetUserInfoFromClaims();

            // Kiểm tra quyền truy cập
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var docFlight = await _context.documents.FindAsync(idDocument);
                if (docFlight == null || docFlight.IsDeleted == true)
                {
                    throw new FileNotFoundException("Document not found");
                }
                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    docFlight.Status = "Confirmed";
                    docFlight.Signature = userInfo.Email;
                    docFlight.UpdatedTime = DateTime.UtcNow;

                    _context.documents.Update(docFlight);
                    await _context.SaveChangesAsync();

                    return docFlight;
                }
                var flight = await _context.flights.FirstOrDefaultAsync(x => x.IdFlight == docFlight.FlightId);
                if (userInfo.IdUser == flight.UserId)
                {
                    docFlight.Status = "Confirmed";
                    docFlight.Signature = userInfo.Email;
                    docFlight.UpdatedTime = DateTime.UtcNow;

                    _context.documents.Update(docFlight);
                    await _context.SaveChangesAsync();

                    return docFlight;
                }
                throw new UnauthorizedAccessException("You do not have permission to confirm this document");
            }
            throw new UnauthorizedAccessException("You do not have permission to confirm this document");
        }

    }
}
