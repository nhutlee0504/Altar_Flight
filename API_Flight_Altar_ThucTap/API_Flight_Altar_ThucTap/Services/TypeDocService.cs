using API_Flight_Altar.Data;
using API_Flight_Altar_ThucTap.Model;
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
        public async Task<TypeDoc> AddTypeDoc(TypeDocDto typeDocDto)
        {
            // Lấy IdUser từ claims
            var userId = GetUserIdFromClaims();

            var typeDoc = new TypeDoc
            {
                TypeName = typeDocDto.TypeDocName,
                CreatedDate = DateTime.UtcNow,
                UserId = userId
            };

            await _context.typeDocs.AddAsync(typeDoc);
            await _context.SaveChangesAsync();

            return typeDoc;
        }

        public async Task<IEnumerable<TypeDoc>> GetTypeDocs()
        {
            return await _context.typeDocs.ToListAsync();
        }

        public async Task<TypeDoc> UpdateTypeDoc(int id, TypeDocDto typeDocDto)
        {
            var typeFind = await _context.typeDocs.FindAsync(id);
            if(typeFind != null)
            {
                typeFind.TypeName = typeDocDto.TypeDocName;
                await _context.SaveChangesAsync();
                return typeFind;
            }
            throw new NotImplementedException("Không tìm thấy loại tài liệu");
        }

        private int GetUserIdFromClaims()
        {
            var userClaim = _httpContextAccessor.HttpContext?.User;
            if (userClaim != null && userClaim.Identity.IsAuthenticated)
            {
                var userIdClaim = userClaim.FindFirst(ClaimTypes.NameIdentifier); // Lấy IdUser từ claim
                if (userIdClaim != null)
                {
                    return int.Parse(userIdClaim.Value);
                }
            }
            throw new UnauthorizedAccessException("Không tìm thấy thông tin người dùng.");
        }
    }
}
