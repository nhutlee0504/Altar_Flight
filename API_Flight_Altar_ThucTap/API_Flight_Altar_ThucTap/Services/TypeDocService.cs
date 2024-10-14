using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Dto;
using API_Flight_Altar_ThucTap.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
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
            throw new UnauthorizedAccessException("Bạn không có quyền tạo loại tài liệu");


        }

        public async Task<TypeDoc> DeleteTypeDoc(int id)//Xóa tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var typeFind = await _context.typeDocs.FindAsync(id);
                if (typeFind == null)
                {
                    throw new NotImplementedException("Không tìm thấy loại tài liệu");
                }
                if (typeFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa loại tài liệu này.");
                }
                typeFind.Status = "Delete";
                await _context.SaveChangesAsync();
                return typeFind;
            }
            throw new UnauthorizedAccessException("Bạn không có quyền xóa tài liệu");
          
        }

        public async Task<TypeDoc> FindTypeDocByName(string name)//Tìm loại tài liệu qua tên
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.TypeName == name);
                if (typeFind != null)
                {
                    return typeFind;
                }
                throw new NotImplementedException("Không tìm thấy loại tài liệu");
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");

        }

        public async Task<IEnumerable<TypeDoc>> GetTypeDocs()//Lấy tất cả loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role == "Admin" || userInfo.Role == "GO")
            {
                return await _context.typeDocs.ToListAsync();
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");

        }

        public async Task<TypeDoc> UpdateTypeDoc(int id, string typeName, string note)//Cập nhật loại tài liệu
        {
            var userInfo = GetUserInfoFromClaims(); // Lấy thông tin người dùng

            if (userInfo.Role != "Admin" || userInfo.Role != "GO")
            {
                var typeFind = await _context.typeDocs.FirstOrDefaultAsync(x => x.IdTypeDoc == id);
                if (typeFind == null)
                {
                    throw new NotImplementedException("Không tìm thấy loại tài liệu");
                }
                if (typeFind.UserId != userInfo.IdUser)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa loại tài liệu này.");
                }
                typeFind.TypeName = typeName;
                typeFind.Note = note;
                await _context.SaveChangesAsync();
                return typeFind;
            }
            throw new UnauthorizedAccessException("Bạn không có quyền thực hiện chức năng.");

        }

        //Phương thức ngoài

        private (int IdUser, string Email, string Role) GetUserInfoFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var idClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier);
                var emailClaim = userClaim.FindFirst(ClaimTypes.Email);
                var roleClaim = userClaim.FindFirst(ClaimTypes.Role);

                if (idClaim != null && emailClaim != null && roleClaim != null)
                {
                    return (int.Parse(idClaim.Value), emailClaim.Value, roleClaim.Value);
                }
            }
            throw new UnauthorizedAccessException("Vui lòng đăng nhập vào hệ thống.");
        }
    }
}
