using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace API_Flight_Altar_ThucTap.Services
{
    public class TypeDocService : ITypeDoc
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TypeDocService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TypeDoc> AddTypeDoc(string typeName, string note)//Tạo loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var typeDoc = new TypeDoc
                {
                    TypeName = typeName,
                    Note = note,
                    CreatedDate = DateTime.UtcNow,
                    UserId = userInfo.IdUser,
                    Status = "Active",
                };

                await _context.typeDocs.AddAsync(typeDoc);
                await _context.SaveChangesAsync();

                return typeDoc;
            }
            throw new UnauthorizedAccessException("You do not have access permission");


        }

        public async Task<TypeDoc> DeleteTypeDoc(int id)//Xóa tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var typeFind = await _context.typeDocs.FindAsync(id);
                if (typeFind == null)
                {
                    throw new NotImplementedException("No typeDoc found");
                }
                if (userInfo.Role == "Admin")
                {
                    typeFind.Status = "Deleted";
                    var gt = await _context.group_Types.Where(x => x.IdType == typeFind.IdTypeDoc).ToListAsync();
                    _context.typeDocs.Remove(typeFind);
                    await _context.SaveChangesAsync();
                    return typeFind;
                }
                if (typeFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("You do not have access permission");
                }
                if (typeFind.Status == "Deleted")
                {
                    throw new UnauthorizedAccessException("The document type has been deleted previously");
                }
                typeFind.Status = "Deleted";
                await _context.SaveChangesAsync();
                return typeFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<TypeDoc> FindTypeDocById(int idTypeDoc)
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == idTypeDoc);
                if (typeFind == null)
                {
                    throw new NotImplementedException("No document type found");
                }
                return typeFind;
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        public async Task<IEnumerable<TypeDoc>> FindTypeDocByName(string name)//Tìm loại tài liệu qua tên
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var typeFind = await _context.typeDocs.Where(x => x.TypeName.Contains(name)).ToListAsync();
                if (typeFind == null)
                {
                    throw new NotImplementedException("No document type found");
                }

                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    return typeFind;
                }
                return typeFind.Where(x => x.Status != "Deleted");
            }
            throw new UnauthorizedAccessException("You do not have access permission");

        }

        public async Task<IEnumerable<TypeDoc>> GetMyTypeDoc()
        {
            var userInfo = GetUserInfoFromClaims();
            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var myType = await _context.typeDocs.Where(x => x.UserId == userInfo.IdUser).ToListAsync();
                if (myType == null)
                {
                    throw new NotImplementedException("No document type found");
                }

                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    return myType;
                }
                return myType.Where(x => x.Status != "Deleted");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TypeDoc>> GetTypeDocs()//Lấy tất cả loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var typeFind = await _context.typeDocs.ToListAsync();
                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    return typeFind;
                }
                return typeFind.Where(x => x.Status != "Deleted");
            }
            throw new UnauthorizedAccessException("You do not have access permission");

        }

        public async Task<TypeDoc> UpdateTypeDoc(int id, string typeName, string note)//Cập nhật loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role.ToLower().Contains("admin") || userInfo.Role.ToLower().Contains("go"))
            {
                var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == id);
                if (typeFind == null)
                {
                    throw new NotImplementedException("No document type found");
                }
                if (typeFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("You do not have access permission");
                }
                if (typeFind.Status == "Deleted")
                {
                    throw new UnauthorizedAccessException("The document type has been deleted");
                }
                if (userInfo.Role.ToLower().Contains("admin"))
                {
                    typeFind.TypeName = typeName;
                    typeFind.Note = note;
                    await _context.SaveChangesAsync();
                    return typeFind;
                }
                if (userInfo.IdUser == typeFind.UserId)
                {
                    typeFind.TypeName = typeName;
                    typeFind.Note = note;
                    await _context.SaveChangesAsync();
                    return typeFind;
                }
                throw new UnauthorizedAccessException("You do not have permission to edit this document type");
            }
            throw new UnauthorizedAccessException("You do not have access permission");
        }

        //Phương thức ngoài

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
